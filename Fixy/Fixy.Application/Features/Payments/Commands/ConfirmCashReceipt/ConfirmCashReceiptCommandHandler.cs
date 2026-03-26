using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;

public class ConfirmCashReceiptCommandHandler : IRequestHandler<ConfirmCashReceiptCommand, Result<ConfirmCashReceiptResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private const decimal PLATFORM_COMMISSION_RATE = 0.15m;

    public ConfirmCashReceiptCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ConfirmCashReceiptResponse>> Handle(ConfirmCashReceiptCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current user (technician)
        var userId = _currentUserService.GetCurrentUserId();

        Log.Information($"Technician {userId} confirming cash receipt for booking {request.BookingId}");

        // 2. Get booking with all related data
        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            Log.Warning($"Booking not found: {request.BookingId}");
            return Errors.BookingNotFound;
        }

        // 3. Verify booking status is PaymentPending
        if (booking.Status != ServiceBookingStatus.PaymentPending)
        {
            Log.Warning($"Booking {request.BookingId} is not awaiting payment. Status: {booking.Status}");
            return Errors.InvalidBookingStatus;
        }

        // 4. Verify payment exists and is cash payment
        if (booking.Payment == null)
        {
            Log.Warning($"Payment not found for booking {request.BookingId}");
            return Errors.PaymentNotFound;
        }

        if (booking.Payment.Method != PaymentMethod.Cash)
        {
            Log.Warning($"Payment for booking {request.BookingId} is not cash payment");
            return Errors.PaymentNotCash;
        }

        // 5. Verify payment is still pending
        if (booking.Payment.Status == PaymentStatus.Success)
        {
            Log.Warning($"Cash already confirmed for booking {request.BookingId}");
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

        await _unitOfWork.TechnicianCommissionsOwed.AddAsync(commissionOwed);

        Log.Information($"Cash payment created - Commission owed: {platformCommission}");

        // 7. Update booking status
        booking.Status = ServiceBookingStatus.Completed;

        await _unitOfWork.SaveChangesAsync();

        Log.Information($"Cash receipt confirmed for booking {request.BookingId}");

        // 8. Send notifications
        //await SendNotificationsAsync(booking, payment, cancellationToken);

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

        return response;
    }
}