using Fixy.Application.Bases;
using Fixy.Domain.Entities.Payments;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed class ProcessCallbackCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymentService paymentService,
    INotificationService notificationService) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
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

            // 3. Find the payment record using OrderReference (mapped from Stripe metadata)
            var payment = await unitOfWork.Payments.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == callbackResult.OrderReference, cancellationToken);

            if (payment == null)
            {
                if (!callbackResult.Success)
                {
                    Log.Warning("Payment not found for failed order: {OrderRef}", callbackResult.OrderReference);
                    return true; // Non-critical, return OK to Stripe
                }

                Log.Warning("Payment not found for successful order: {OrderRef}", callbackResult.OrderReference);
                return Errors.PaymentNotFound;
            }

            // 4. Idempotency guard — Stripe can deliver webhooks more than once
            if (payment.Status == PaymentStatus.Success && callbackResult.Success)
            {
                Log.Information("Payment already processed: {OrderRef}", callbackResult.OrderReference);
                return true;
            }

            // 5. Update payment record
            if (callbackResult.Success)
            {
                payment.PaymentIntentId = callbackResult.PaymentIntentId;
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
                if (callbackResult.OrderReference.StartsWith("BK-"))
                    await HandleBookingPaymentSuccessAsync(payment, cancellationToken);

                else if (callbackResult.OrderReference.StartsWith("COMM-"))
                    await HandleCommissionPaymentSuccessAsync(payment, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync();

            Log.Information("Callback processed: {OrderRef} → {Status}",
                callbackResult.OrderReference, callbackResult.Status);

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

    // ---------------------------------------------------------------
    // Booking Payment Success
    // ---------------------------------------------------------------
    private async Task HandleBookingPaymentSuccessAsync(Payment payment, CancellationToken cancellationToken)
    {
        Log.Information("Processing successful booking payment: {OrderRef}", payment.MerchantOrderId);

        if (!payment.ServiceBookingId.HasValue)
        {
            Log.Warning("Booking payment has no ServiceBookingId: {PaymentId}", payment.Id);
            return;
        }

        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician)
            .Include(x => x.ServiceRequest)
                .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value, cancellationToken);

        if (booking == null)
        {
            Log.Warning("Booking not found: {BookingId}", payment.ServiceBookingId);
            return;
        }

        // Update booking & counters
        booking.Status = ServiceBookingStatus.Completed;
        booking.Technician.CompletedBookings += 1;
        booking.ServiceRequest.Customer.CompletedBookings += 1;

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

        // Notify customer
        await notificationService.SendFullNotificationAsync(
            booking.ServiceRequest.Customer,
            NotificationType.BookingCompleted,
            SharedResourcesKeys.NotificationBookingCompletedTitle,
            SharedResourcesKeys.NotificationBookingCompletedBody
        );

        Log.Information("Payout created: {Amount:C} for technician {TechnicianId}",
            payment.TechnicianAmount, booking.TechnicianId);
    }

    // ---------------------------------------------------------------
    // Commission Payment Success
    // ---------------------------------------------------------------
    private async Task HandleCommissionPaymentSuccessAsync(Payment payment, CancellationToken cancellationToken)
    {
        Log.Information("Processing successful commission payment: {OrderRef}", payment.MerchantOrderId);

        // Format: COMM-{guid}-{randomchars}
        // GUID has 5 parts: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        var parts = payment.MerchantOrderId.Split('-');

        if (parts.Length < 6)
        {
            Log.Error("Invalid merchant order ID format: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        // Reconstruct GUID from parts 1–5
        var technicianIdString = $"{parts[1]}-{parts[2]}-{parts[3]}-{parts[4]}-{parts[5]}";

        if (!Guid.TryParse(technicianIdString, out var technicianId))
        {
            Log.Error("Cannot extract technician ID from: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        Log.Information("Extracted technician ID: {TechnicianId}", technicianId);

        // Get all unpaid commissions for this technician
        var commissions = await unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
            .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
            .ToListAsync(cancellationToken);

        if (!commissions.Any())
        {
            Log.Warning("No unpaid commissions found for technician {TechnicianId}", technicianId);
            return;
        }

        foreach (var commission in commissions)
        {
            commission.IsPaid = true;
            commission.PaidAt = DateTime.UtcNow;
        }

        Log.Information("Marked {Count} commissions as paid for technician {TechnicianId}",
            commissions.Count, technicianId);
    }
}