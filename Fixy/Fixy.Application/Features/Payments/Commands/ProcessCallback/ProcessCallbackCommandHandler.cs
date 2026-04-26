using Fixy.Application.Bases;
using Fixy.Domain.Entities.Payments;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed class ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Delegate ALL Stripe logic to the service
            var callbackResult = await paymentService.ProcessCallbackAsync();

            // 2. Handle unrecognized/unsubscribed events gracefully
            if (callbackResult == null)
                return true; // Stripe expects 200 OK for unhandled events

            // 3. Find the payment record
            var payment = await unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == callbackResult.MerchantOrderId, cancellationToken);

            if (payment == null)
            {
                // For failures, missing payment is non-critical — still return OK to Stripe
                if (!callbackResult.Success)
                {
                    Log.Warning("Payment not found for failed/expired order: {OrderId}", callbackResult.MerchantOrderId);
                    return true;
                }

                Log.Warning("Payment not found for successful order: {OrderId}", callbackResult.MerchantOrderId);
                return Errors.PaymentNotFound;
            }

            // 4. Idempotency guard — Stripe can deliver webhooks more than once
            if (payment.Status == PaymentStatus.Success && callbackResult.Success)
            {
                Log.Information("Payment already processed: {OrderId}", callbackResult.MerchantOrderId);
                return true;
            }

            // 5. Update payment record
            if (callbackResult.Success)
            {
                payment.StripeSessionId = callbackResult.Metadata?.GetValueOrDefault("session_id")?.ToString();
                payment.PaymentIntentId = callbackResult.TransactionId;
                payment.Status = PaymentStatus.Success;
                payment.PaidAt = DateTime.UtcNow;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                payment.PaidAt = null;
            }

            // 6. Run domain side-effects only on success
            if (callbackResult.Success)
            {
                if (callbackResult.MerchantOrderId.StartsWith("BK-"))
                    await HandleBookingPaymentSuccessAsync(payment, cancellationToken);

                else if (callbackResult.MerchantOrderId.StartsWith("COMM-"))
                    await HandleCommissionPaymentSuccessAsync(payment, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync();

            Log.Information("Callback processed: {OrderId} → {Status}",
                callbackResult.MerchantOrderId, callbackResult.Status);

            return true;
        }
        catch (Exception ex) when (ex.Message == "Invalid webhook signature")
        {
            return Errors.InvalidHmacSignature;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing payment callback");
            return Errors.CallbackProcessingFailed;
        }
    }
    
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
        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.Technician)
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value, cancellationToken);

        if (booking == null)
        {
            Log.Warning($"Booking not found: {payment.ServiceBookingId}");
            return;
        }

        // Update booking status
        booking.Status = ServiceBookingStatus.Completed;
        booking.Technician.TotalCompletedJobs+=1;

        Log.Information($"Booking {booking.Id} marked as Paid");

        // Create payout for technician
        var payout = new Domain.Entities.Payments.Payout
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

    private async Task HandleCommissionPaymentSuccessAsync(Payment payment, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //Log.Information($"Processing successful commission payment: {payment.MerchantOrderId}");

        //// Extract technician ID from merchant order (COMM-{technicianId}-{random})
        //// Format: COMM-guid-randomchars
        //// Guid format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx (5 parts)
        //var parts = payment.MerchantOrderId.Split('-');

        //if (parts.Length < 6)
        //{
        //    Log.Error($"Invalid merchant order ID format: {payment.MerchantOrderId}");
        //    return;
        //}

        //// Reconstruct GUID from parts 1-5
        //var technicianIdString = $"{parts[1]}-{parts[2]}-{parts[3]}-{parts[4]}-{parts[5]}";

        //if (!Guid.TryParse(technicianIdString, out var technicianId))
        //{
        //    Log.Error($"Cannot extract technician ID from: {payment.MerchantOrderId}");
        //    return;
        //}

        //Log.Information($"Extracted technician ID: {technicianId}");

        //// Get all unpaid commissions for this technician
        ////var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
        //    .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
        //    .ToListAsync(cancellationToken);

        //if (!commissions.Any())
        //{
        //    Log.Warning($"No unpaid commissions found for technician {technicianId}");
        //    return;
        //}

        //// Mark all as paid
        //foreach (var commission in commissions)
        //{
        //    commission.IsPaid = true;
        //    commission.PaidAt = DateTime.UtcNow;
        //    //await unitOfWork.TechnicianCommissionsOwed.UpdateAsync(commission);
        //}

        //Log.Information($"Marked {commissions.Count} commissions as paid for technician {technicianId}");
    }
}