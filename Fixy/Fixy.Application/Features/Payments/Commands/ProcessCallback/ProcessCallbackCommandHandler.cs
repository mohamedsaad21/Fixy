using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed class ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, IPaymobService paymobService) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var query = request.QueryData;

            // Extract data from query string
            var transactionId = query["id"].ToString();
            var success = query["success"].ToString().ToLower() == "true";
            var amountCents = query["amount_cents"].ToString();
            var merchantOrderId = query["merchant_order_id"].ToString();
            var hmac = query["hmac"].ToString();

            Log.Information($"Processing callback - Order: {merchantOrderId}, Success: {success}");

            //Verify HMAC(enable in production)
            //if (!paymobService.VerifyHmac(query, hmac))
            //{
            //    Log.Error($"Invalid HMAC signature for order: {merchantOrderId}");
            //    return Errors.InvalidHmacSignature;
            //}

            // Find payment by MerchantOrderId
            var payment = await unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId, cancellationToken);

            if (payment == null)
            {
                Log.Warning($"Payment not found for order: {merchantOrderId}");
                return Errors.PaymentNotFound;
            }

            // Check if already processed
            if (payment.Status == PaymentStatus.Success && !string.IsNullOrEmpty(payment.PaymobTransactionId))
            {
                Log.Information($"Payment already processed: {merchantOrderId}");
                return true;
            }

            // Update payment
            payment.PaymobTransactionId = transactionId;
            payment.PaymobOrderId = query["order"].ToString();
            payment.Status = success ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.PaidAt = success ? DateTime.UtcNow : null;

            // Handle based on payment type
            if (success)
            {
                if (merchantOrderId.StartsWith("BK-"))
                {
                    // BOOKING PAYMENT
                    await HandleBookingPaymentSuccessAsync(payment, cancellationToken);
                }
                else if (merchantOrderId.StartsWith("COMM-"))
                {
                    // COMMISSION PAYMENT
                    await HandleCommissionPaymentSuccessAsync(payment, cancellationToken);
                }
                else
                {
                    Log.Warning($"Unknown payment type: {merchantOrderId}");
                }
            }
            else
            {
                Log.Warning($"Payment failed: {merchantOrderId}");
            }

            await unitOfWork.SaveChangesAsync();

            Log.Information($"Callback processed successfully: {merchantOrderId}");

            return success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing callback");
            return Errors.CallbackProcessingFailed;
        }
    }

    // BOOKING PAYMENT SUCCESS
    private async Task HandleBookingPaymentSuccessAsync(Payment payment, CancellationToken cancellationToken)
    {
        Log.Information($"Processing successful booking payment: {payment.MerchantOrderId}");

        // Check if payment has a booking
        if (!payment.ServiceBookingId.HasValue)
        {
            Log.Warning($"Booking payment has no ServiceBookingId: {payment.Id}");
            return;
        }

        // Get booking
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value, cancellationToken);

        if (booking == null)
        {
            Log.Warning($"Booking not found: {payment.ServiceBookingId}");
            return;
        }

        // Update booking status
        booking.Status = ServiceBookingStatus.Completed;

        Log.Information($"Booking {booking.Id} marked as Paid");

        // Create payout for technician
        var payout = new Payout
        {
            TechnicianId = booking.TechnicianId,
            BookingId = booking.Id,
            Amount = payment.TechnicianAmount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Payouts.AddAsync(payout);

        Log.Information($"Payout created: {payment.TechnicianAmount:C} for technician {booking.TechnicianId}");
    }

    // COMMISSION PAYMENT SUCCESS
    private async Task HandleCommissionPaymentSuccessAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        Log.Information($"Processing successful commission payment: {payment.MerchantOrderId}");

        // Extract technician ID from merchant order (COMM-{technicianId}-{random})
        var parts = payment.MerchantOrderId.Split('-');
        if (parts.Length < 2 || !Guid.TryParse(parts[1] + parts[2] + parts[3] + parts[4] + parts[5], out var technicianId))
        {
            Log.Error($"Cannot extract technician ID from: {payment.MerchantOrderId}");
            return;
        }

        // Get all unpaid commissions for this technician
        var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
            .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
            .ToListAsync(cancellationToken);

        if (!commissions.Any())
        {
            Log.Warning($"No unpaid commissions found for technician {technicianId}");
            return;
        }

        // Mark all as paid
        foreach (var commission in commissions)
        {
            commission.IsPaid = true;
            commission.PaidAt = DateTime.UtcNow;
        }

        Log.Information($"Marked {commissions.Count} commissions as paid for technician {technicianId}");
    }
}