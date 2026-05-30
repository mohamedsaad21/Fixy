using Fixy.Application.Common.DTOs.Payment;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Stripe;

public class StripeService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public StripeService(
        StripeSettings stripeSettings,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _stripeSettings = stripeSettings;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;

        StripeConfiguration.ApiKey = _stripeSettings.Secretkey;
    }

    public async Task<PaymentUrlResult> CreatePaymentUrlAsync(
        decimal amount, Guid referenceId, string customerName,
        string customerEmail, string customerPhone, string orderPrefix = "BK")
    {
        try
        {
            var orderReference = $"{orderPrefix}-{referenceId.ToString().ToUpper()}";
            var customerId = await GetOrCreateCustomerAsync(customerName, customerEmail, customerPhone);

            var options = new PaymentIntentCreateOptions
            {
                Amount = ConvertToSmallestUnit(amount),
                Currency = "usd",
                Customer = customerId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    { "order_reference", orderReference },
                    { "reference_id",    referenceId.ToString() },
                    { "customer_name",   customerName },
                    { "customer_email",  customerEmail },
                    { "customer_phone",  customerPhone },
                },
                Description = $"Order {orderReference} for {customerName}",
                ReceiptEmail = customerEmail,
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            Log.Information("PaymentIntent created: {Id} for order {Order}",
                paymentIntent.Id, orderReference);

            return new PaymentUrlResult
            {
                Success = true,
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id,
                MerchantOrderId = orderReference,
            };
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Stripe error creating PaymentIntent");
            return new PaymentUrlResult { Success = false, ErrorMessage = ex.StripeError?.Message ?? ex.Message };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error creating PaymentIntent");
            return new PaymentUrlResult { Success = false, ErrorMessage = "An unexpected error occurred." };
        }
    }

    public async Task<bool> VerifyWebhookSignature(string payload, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload, signature, _stripeSettings.WebhookSecret);

            Log.Information("Webhook received: {Type}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentSucceededAsync(succeededIntent);
                    break;

                case "payment_intent.payment_failed":
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentFailedAsync(failedIntent);
                    break;

                case "charge.refunded":
                    var charge = stripeEvent.Data.Object as Charge;
                    await HandleRefundAsync(charge);
                    break;

                default:
                    Log.Information("Unhandled webhook event: {Type}", stripeEvent.Type);
                    break;
            }

            return true;
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Webhook signature verification failed");
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error handling webhook");
            return false;
        }
    }

    private async Task HandlePaymentSucceededAsync(PaymentIntent intent)
    {
        var orderReference = intent.Metadata.GetValueOrDefault("order_reference");

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.MerchantOrderId == orderReference);

        if (payment == null)
        {
            Log.Warning("Webhook — payment not found: {Order}", orderReference);
            return;
        }

        if (payment.Status == PaymentStatus.Success)
        {
            Log.Information("Webhook — already processed: {Order}", orderReference);
            return;
        }

        payment.PaymentIntentId = intent.Id;
        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;

        if (orderReference.StartsWith("BK-"))
            await HandleBookingPaymentSuccessAsync(payment);
        else if (orderReference.StartsWith("COMM-"))
            await HandleCommissionPaymentSuccessAsync(payment);

        await _unitOfWork.SaveChangesAsync();

        Log.Information("Webhook — payment updated: {Order} → Success", orderReference);
    }

    private async Task HandlePaymentFailedAsync(PaymentIntent intent)
    {
        var orderReference = intent.Metadata.GetValueOrDefault("order_reference");

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.MerchantOrderId == orderReference);

        if (payment == null)
        {
            Log.Warning("Webhook — payment not found for failed order: {Order}", orderReference);
            return;
        }

        if (payment.Status == PaymentStatus.Success)
        {
            Log.Information("Webhook — ignoring failed, already succeeded: {Order}", orderReference);
            return;
        }

        payment.Status = PaymentStatus.Failed;
        payment.PaidAt = null;

        await _unitOfWork.SaveChangesAsync();

        Log.Warning("Webhook — payment updated: {Order} → Failed", orderReference);
    }

    private async Task HandleRefundAsync(Charge charge)
    {
        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.PaymentIntentId == charge.PaymentIntentId);

        if (payment == null)
        {
            Log.Warning("Webhook — payment not found for charge: {Id}", charge.Id);
            return;
        }

        payment.Status = PaymentStatus.Refunded;
        payment.PaidAt = null;

        await _unitOfWork.SaveChangesAsync();

        Log.Information("Webhook — payment updated: {Order} → Refunded", payment.MerchantOrderId);
    }

    private async Task HandleBookingPaymentSuccessAsync(Payment payment)
    {
        if (!payment.ServiceBookingId.HasValue)
        {
            Log.Warning("No ServiceBookingId on payment: {PaymentId}", payment.Id);
            return;
        }

        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician)
            .Include(x => x.ServiceRequest)
                .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value);

        if (booking == null)
        {
            Log.Warning("Booking not found: {BookingId}", payment.ServiceBookingId);
            return;
        }

        booking.Status = ServiceBookingStatus.Completed;
        booking.Technician.CompletedBookings += 1;
        booking.ServiceRequest.Customer.CompletedBookings += 1;

        await _unitOfWork.Payouts.AddAsync(new Fixy.Domain.Entities.Payments.Payout
        {
            TechnicianId = booking.TechnicianId,
            BookingId = booking.Id,
            Amount = payment.TechnicianAmount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        await _notificationService.SendFullNotificationAsync(
            booking.ServiceRequest.Customer,
            NotificationType.BookingCompleted,
            SharedResourcesKeys.NotificationBookingCompletedTitle,
            SharedResourcesKeys.NotificationBookingCompletedBody
        );

        Log.Information("Payout created: {Amount:C} for technician {TechnicianId}",
            payment.TechnicianAmount, booking.TechnicianId);
    }

    private async Task HandleCommissionPaymentSuccessAsync(Payment payment)
    {
        var parts = payment.MerchantOrderId.Split('-');

        if (parts.Length < 6)
        {
            Log.Error("Invalid order format: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        var technicianIdString = $"{parts[1]}-{parts[2]}-{parts[3]}-{parts[4]}-{parts[5]}";

        if (!Guid.TryParse(technicianIdString, out var technicianId))
        {
            Log.Error("Cannot extract technician ID: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        var commissions = await _unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
            .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
            .ToListAsync();

        if (!commissions.Any())
        {
            Log.Warning("No unpaid commissions for technician {TechnicianId}", technicianId);
            return;
        }

        foreach (var commission in commissions)
        {
            commission.IsPaid = true;
            commission.PaidAt = DateTime.UtcNow;
        }

        Log.Information("Marked {Count} commissions paid for technician {TechnicianId}",
            commissions.Count, technicianId);
    }

    private async Task<string> GetOrCreateCustomerAsync(string name, string email, string phone)
    {
        var searchOptions = new CustomerSearchOptions { Query = $"email:'{email}'" };
        var searchService = new CustomerService();
        var existing = await searchService.SearchAsync(searchOptions);

        if (existing.Data.Any())
            return existing.Data.First().Id;

        var createOptions = new CustomerCreateOptions { Name = name, Email = email, Phone = phone };
        var customer = await searchService.CreateAsync(createOptions);
        return customer.Id;
    }

    private long ConvertToSmallestUnit(decimal amount)
        => (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
}