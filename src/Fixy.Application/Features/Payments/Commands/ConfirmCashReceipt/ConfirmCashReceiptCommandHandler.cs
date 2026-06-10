using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt.Responses;
using Fixy.Application.Resources;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;

public class ConfirmCashReceiptCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
INotificationService notificationService, ILogger<ConfirmCashReceiptCommandHandler> logger) : IRequestHandler<ConfirmCashReceiptCommand, Result<ConfirmCashReceiptResponse>>
{    private const decimal PLATFORM_COMMISSION_RATE = 0.10m;
    public async Task<Result<ConfirmCashReceiptResponse>> Handle(ConfirmCashReceiptCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user (technician)
        var userId = currentUserService.GetCurrentUserId();

        logger.LogInformation($"Technician {userId} confirming cash receipt for booking {request.BookingId}");

        // 2. Get booking with all related data
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(b => b.Payment).Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            logger.LogWarning($"Booking not found: {request.BookingId}");
            return Errors.BookingNotFound;
        }

        // 3. Verify booking status is PaymentPending
        if (booking.Status != ServiceBookingStatus.AwaitingPayment)
        {
            logger.LogWarning($"Booking {request.BookingId} is not awaiting payment. Status: {booking.Status}");
            return Errors.InvalidBookingStatus;
        }

        // 4. Verify payment exists and is cash payment
        if (booking.Payment == null)
        {
            logger.LogWarning($"Payment not found for booking {request.BookingId}");
            return Errors.PaymentNotFound;
        }

        if (booking.Payment.Method != PaymentMethod.Cash)
        {
            logger.LogWarning($"Payment for booking {request.BookingId} is not cash payment");
            return Errors.PaymentNotCash;
        }

        // 5. Verify payment is still pending
        if (booking.Payment.Status == PaymentStatus.Success)
        {
            logger.LogWarning($"Cash already confirmed for booking {request.BookingId}");
            return Errors.PaymentAlreadyCompleted;
        }

        // 6. Update payment status
        var payment = booking.Payment;
        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;

        var totalAmount = booking.AgreedPrice;
        var platformCommission = totalAmount * PLATFORM_COMMISSION_RATE;

        // Create TechnicianCommissionOwed record
        var commissionOwed = new TechnicianCommissionOwed
        {
            TechnicianId = booking.TechnicianId,
            BookingId = booking.Id,
            AmountOwed = platformCommission,
            IsPaid = false,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.TechnicianCommissionsOwed.AddAsync(commissionOwed);

        logger.LogInformation($"Cash payment created - Commission owed: {platformCommission}");

        // 7. Update booking status
        booking.Status = ServiceBookingStatus.AwaitingFeedback;
        booking.Technician.CompletedBookings += 1;
        booking.ServiceRequest.Customer.CompletedBookings += 1;

        logger.LogInformation($"Cash receipt confirmed for booking {request.BookingId}");

        // 8. Send notifications
        var customer = booking.ServiceRequest.Customer;
        await unitOfWork.SaveChangesAsync();

        // 9. Build response
        var response = new ConfirmCashReceiptResponse
        {
            PaymentId = payment.Id,
            BookingId = booking.Id,
            AmountConfirmed = payment.TotalAmount,
            TechnicianEarning = payment.TechnicianAmount,
            PlatformCommission = payment.PlatformCommission,
            Status = "Success",
            Message = $"Cash receipt confirmed. You earned {payment.TechnicianAmount:C}. You owe platform: {payment.PlatformCommission:C}"
        };

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            customer.Id,
            NotificationType.BookingCompleted,
            SharedResourcesKeys.NotificationBookingCompletedTitle,
            SharedResourcesKeys.NotificationBookingCompletedBody
        ));

        return response;
    }
}