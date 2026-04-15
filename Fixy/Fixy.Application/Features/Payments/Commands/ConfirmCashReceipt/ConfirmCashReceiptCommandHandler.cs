using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt.Responses;
using Fixy.Domain.Constants;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;

public class ConfirmCashReceiptCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<ConfirmCashReceiptCommand, Result<ConfirmCashReceiptResponse>>
{
    public async Task<Result<ConfirmCashReceiptResponse>> Handle(ConfirmCashReceiptCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        Log.Information($"Technician {userId} confirming cash receipt for booking {request.BookingId}");

        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            Log.Warning($"Booking not found: {request.BookingId}");
            return Errors.BookingNotFound;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingPayment)
        {
            Log.Warning($"Booking {request.BookingId} is not awaiting payment. Status: {booking.Status}");
            return Errors.InvalidBookingStatus;
        }

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

        if (booking.Payment.Status == PaymentStatus.Success)
        {
            Log.Warning($"Cash already confirmed for booking {request.BookingId}");
            return Errors.PaymentAlreadyCompleted;
        }

        var technicianWallet = await unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == booking.TechnicianId);

        if (technicianWallet == null)
            return Errors.TechnicianWalletNotFound;

        var payment = booking.Payment;
        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;
        booking.Status = ServiceBookingStatus.Completed;

        technicianWallet.Balance -= booking.AgreedPrice * WalletConstants.CommissionRate;
        technicianWallet.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        Log.Information($"Cash receipt confirmed for booking {request.BookingId}");

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