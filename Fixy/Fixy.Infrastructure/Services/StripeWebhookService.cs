using Fixy.Application.Abstracts;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeSettings _stripeSettings;

    public StripeWebhookService(IUnitOfWork unitOfWork, StripeSettings stripeSettings)
    {
        _unitOfWork = unitOfWork;
        _stripeSettings = stripeSettings;
    }

    public async Task<bool> HandleWebhookAsync(string json, string signature)
    {
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                _stripeSettings.WebhookSecret
            );
        }
        catch (Exception)
        {
            return false;
        }

        // Check for duplicate events (idempotency)
        var existingEvent = await _unitOfWork.StripeWebhookEvents.GetTableNoTracking()
            .FirstOrDefaultAsync(e => e.StripeEventId == stripeEvent.Id);

        if (existingEvent != null && existingEvent.Processed)
        {
            return true;
        }

        // Log the event
        var webhookEvent = new StripeWebhookEvent
        {
            StripeEventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            ReceivedAt = DateTime.UtcNow,
            EventData = json
        };

        await _unitOfWork.StripeWebhookEvents.AddAsync(webhookEvent);
        await _unitOfWork.SaveChangesAsync();

        // Handle different event types
        try
        {
            // ⭐ OPTION 1: Use string constants (RECOMMENDED - No namespace issues)
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    await HandlePaymentSucceededAsync(stripeEvent);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailedAsync(stripeEvent);
                    break;

                case "transfer.created":
                    await HandleTransferCreatedAsync(stripeEvent);
                    break;

                case "transfer.paid":
                    await HandleTransferPaidAsync(stripeEvent);
                    break;

                case "transfer.reversed":
                    await HandleTransferReversedAsync(stripeEvent);
                    break;

                case "charge.refunded":
                    await HandleRefundAsync(stripeEvent);
                    break;

                case "account.updated":
                    await HandleAccountUpdatedAsync(stripeEvent);
                    break;

                case "charge.dispute.created":
                    await HandleDisputeCreatedAsync(stripeEvent);
                    break;

                case "charge.dispute.closed":
                    await HandleDisputeClosedAsync(stripeEvent);
                    break;
            }

            webhookEvent.Processed = true;
            webhookEvent.ProcessedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook processing error: {ex.Message}");
            return false;
        }
    }

    private async Task HandlePaymentSucceededAsync(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null) return;

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .Include(p => p.ServiceBooking)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id);

        if (payment != null)
        {
            payment.Status = "succeeded";
            payment.PaidAt = DateTime.UtcNow;

            if (payment.ServiceBooking != null)
            {
                payment.ServiceBooking.Status = ServiceBookingStatus.Active;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandlePaymentFailedAsync(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null) return;

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .Include(p => p.ServiceBooking)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id);

        if (payment != null)
        {
            payment.Status = "failed";

            if (payment.ServiceBooking != null)
            {
                payment.ServiceBooking.Status = ServiceBookingStatus.PaymentFailed;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleTransferCreatedAsync(Event stripeEvent)
    {
        var transfer = stripeEvent.Data.Object as Transfer;
        if (transfer == null) return;

        var transferRecord = await _unitOfWork.TechnicianTransfers.GetTableAsTracking()
            .FirstOrDefaultAsync(t => t.StripeTransferId == transfer.Id);

        if (transferRecord != null)
        {
            transferRecord.Status = TransferStatus.Pending;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleTransferPaidAsync(Event stripeEvent)
    {
        var transfer = stripeEvent.Data.Object as Transfer;
        if (transfer == null) return;

        var transferRecord = await _unitOfWork.TechnicianTransfers.GetTableAsTracking()
            .Include(t => t.ServiceBooking)
            .Include(t => t.Technician)
            .FirstOrDefaultAsync(t => t.StripeTransferId == transfer.Id);

        if (transferRecord != null)
        {
            transferRecord.Status = TransferStatus.Paid;
            transferRecord.PaidAt = DateTime.UtcNow;

            if (transferRecord.ServiceBooking != null)
            {
                transferRecord.ServiceBooking.Status = ServiceBookingStatus.PaymentRecieved;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleTransferReversedAsync(Event stripeEvent)
    {
        var transfer = stripeEvent.Data.Object as Transfer;
        if (transfer == null) return;

        var transferRecord = await _unitOfWork.TechnicianTransfers.GetTableAsTracking()
            .FirstOrDefaultAsync(t => t.StripeTransferId == transfer.Id);

        if (transferRecord != null)
        {
            transferRecord.Status = TransferStatus.Reversed;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleRefundAsync(Event stripeEvent)
    {
        var charge = stripeEvent.Data.Object as Charge;
        if (charge == null) return;

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .Include(p => p.ServiceBooking)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == charge.PaymentIntentId);

        if (payment != null)
        {
            payment.Status = "refunded";

            var refund = new PaymentRefund
            {
                PaymentId = payment.Id,
                StripeRefundId = charge.Refunds?.Data?.FirstOrDefault()?.Id ?? "",
                Amount = charge.AmountRefunded / 100m,
                Status = "succeeded",
                Reason = "Service not completed or cancelled",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PaymentRefunds.AddAsync(refund);

            if (payment.ServiceBooking != null)
            {
                payment.ServiceBooking.Status = ServiceBookingStatus.Refunded;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleAccountUpdatedAsync(Event stripeEvent)
    {
        var account = stripeEvent.Data.Object as Account;
        if (account == null) return;

        var technicianAccount = await _unitOfWork.TechnicianStripeAccounts.GetTableAsTracking()
            .FirstOrDefaultAsync(a => a.StripeAccountId == account.Id);

        if (technicianAccount != null)
        {
            technicianAccount.ChargesEnabled = account.ChargesEnabled;
            technicianAccount.PayoutsEnabled = account.PayoutsEnabled;
            technicianAccount.OnboardingStatus = account.DetailsSubmitted ? "completed" : "pending";
            technicianAccount.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleDisputeCreatedAsync(Event stripeEvent)
    {
        var dispute = stripeEvent.Data.Object as Stripe.Dispute;
        if (dispute == null) return;

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .Include(p => p.ServiceBooking)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == dispute.PaymentIntentId);

        if (payment?.ServiceBooking != null)
        {
            var disputeRecord = new Domain.Entities.Dispute
            {
                ServiceBookingId = payment.ServiceBookingId,
                StripeDisputeId = dispute.Id,
                Status = "open",
                Reason = dispute.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Disputes.AddAsync(disputeRecord);
            payment.ServiceBooking.Status = ServiceBookingStatus.Disputed;

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleDisputeClosedAsync(Event stripeEvent)
    {
        var dispute = stripeEvent.Data.Object as Stripe.Dispute;
        if (dispute == null) return;

        var disputeRecord = await _unitOfWork.Disputes.GetTableAsTracking()
            .FirstOrDefaultAsync(d => d.StripeDisputeId == dispute.Id);

        if (disputeRecord != null)
        {
            disputeRecord.Status = dispute.Status;
            disputeRecord.ResolvedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
