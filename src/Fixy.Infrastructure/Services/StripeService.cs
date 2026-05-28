using Fixy.Application.Common.DTOs.Payment;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripeService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public StripeService(
        StripeSettings stripeSettings,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _stripeSettings      = stripeSettings;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork          = unitOfWork;
        _notificationService = notificationService;

        StripeConfiguration.ApiKey = _stripeSettings.Secretkey;
    }

    // ---------------------------------------------------------------
    // CreatePaymentUrlAsync — unchanged
    // ---------------------------------------------------------------
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
                Amount   = ConvertToSmallestUnit(amount),
                Currency = "usd",
                Customer = customerId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "order_reference", orderReference },
                    { "reference_id",    referenceId.ToString() },
                    { "customer_name",   customerName },
                    { "customer_email",  customerEmail },
                    { "customer_phone",  customerPhone },
                },
                Description  = $"Order {orderReference} for {customerName}",
                ReceiptEmail = customerEmail,
            };

            var service       = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            Log.Information("PaymentIntent created: {Id} for order {Order}",
                paymentIntent.Id, orderReference);

            return new PaymentUrlResult
            {
                Success         = true,
                ClientSecret    = paymentIntent.ClientSecret,
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

    // ---------------------------------------------------------------
    // ProcessCallbackAsync — unchanged
    // ---------------------------------------------------------------
    public async Task<PaymentCallbackResult> ProcessCallbackAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var request     = httpContext.Request;
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            var callbackData = System.Text.Json.JsonSerializer.Deserialize<CallbackRequest>(
                body, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (string.IsNullOrEmpty(callbackData?.PaymentIntentId))
                return new PaymentCallbackResult { Success = false, ErrorMessage = "PaymentIntentId is required." };

            var service       = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(callbackData.PaymentIntentId);

            var orderReference = paymentIntent.Metadata.GetValueOrDefault("order_reference");
            var amount         = paymentIntent.Amount / 100m;
            var customerEmail  = paymentIntent.Metadata.GetValueOrDefault("customer_email");

            if (paymentIntent.Status == "succeeded")
            {
                Log.Information("Payment succeeded for order {Order}", orderReference);
                return new PaymentCallbackResult
                {
                    Success         = true,
                    PaymentIntentId = paymentIntent.Id,
                    OrderReference  = orderReference,
                    Status          = "succeeded",
                    Amount          = amount,
                    CustomerEmail   = customerEmail,
                };
            }

            return new PaymentCallbackResult
            {
                Success         = false,
                PaymentIntentId = paymentIntent.Id,
                OrderReference  = orderReference,
                Status          = paymentIntent.Status,
                Amount          = amount,
                ErrorMessage    = $"Payment status: {paymentIntent.Status}"
            };
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Stripe error processing callback");
            return new PaymentCallbackResult { Success = false, ErrorMessage = ex.StripeError?.Message ?? ex.Message };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error processing callback");
            return new PaymentCallbackResult { Success = false, ErrorMessage = "An unexpected error occurred." };
        }
    }

    // ---------------------------------------------------------------
    // VerifyWebhookSignature — COMPLETE IMPLEMENTATION
    // ---------------------------------------------------------------
    public async Task<bool> VerifyWebhookSignature(string payload, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                _stripeSettings.WebhookSecret
            );

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
                    Log.Information("Unhandled webhook event type: {Type}", stripeEvent.Type);
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

    // ---------------------------------------------------------------
    // Webhook: Payment Succeeded
    // ---------------------------------------------------------------
    private async Task HandlePaymentSucceededAsync(PaymentIntent intent)
    {
        var orderReference = intent.Metadata.GetValueOrDefault("order_reference");

        Log.Information("Webhook — payment succeeded: {Id}, Order: {Order}",
            intent.Id, orderReference);

        // 1. Find payment record
        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.MerchantOrderId == orderReference);

        if (payment == null)
        {
            Log.Warning("Webhook — payment record not found for order: {Order}", orderReference);
            return;
        }

        // 2. Idempotency guard
        if (payment.Status == PaymentStatus.Success)
        {
            Log.Information("Webhook — payment already processed: {Order}", orderReference);
            return;
        }

        // 3. Update payment record
        payment.PaymentIntentId = intent.Id;
        payment.Status          = PaymentStatus.Success;
        payment.PaidAt          = DateTime.UtcNow;

        // 4. Run domain side-effects
        if (orderReference.StartsWith("BK-"))
            await HandleBookingPaymentSuccessAsync(payment);

        else if (orderReference.StartsWith("COMM-"))
            await HandleCommissionPaymentSuccessAsync(payment);

        await _unitOfWork.SaveChangesAsync();

        Log.Information("Webhook — payment record updated: {Order} → Success", orderReference);
    }

    // ---------------------------------------------------------------
    // Webhook: Payment Failed
    // ---------------------------------------------------------------
    private async Task HandlePaymentFailedAsync(PaymentIntent intent)
    {
        var orderReference = intent.Metadata.GetValueOrDefault("order_reference");

        Log.Warning("Webhook — payment failed: {Id}, Order: {Order}",
            intent.Id, orderReference);

        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.MerchantOrderId == orderReference);

        if (payment == null)
        {
            Log.Warning("Webhook — payment record not found for failed order: {Order}", orderReference);
            return;
        }

        // Don't overwrite a Success status — Stripe can send failed before succeeded
        if (payment.Status == PaymentStatus.Success)
        {
            Log.Information("Webhook — ignoring failed event, payment already succeeded: {Order}", orderReference);
            return;
        }

        payment.Status = PaymentStatus.Failed;
        payment.PaidAt = null;

        await _unitOfWork.SaveChangesAsync();

        Log.Warning("Webhook — payment record updated: {Order} → Failed", orderReference);
    }

    // ---------------------------------------------------------------
    // Webhook: Charge Refunded
    // ---------------------------------------------------------------
    private async Task HandleRefundAsync(Charge charge)
    {
        Log.Information("Webhook — charge refunded: {Id}", charge.Id);

        // Find payment by PaymentIntentId
        var payment = await _unitOfWork.Payments.GetTableAsTracking()
            .FirstOrDefaultAsync(p => p.PaymentIntentId == charge.PaymentIntentId);

        if (payment == null)
        {
            Log.Warning("Webhook — payment record not found for charge: {Id}", charge.Id);
            return;
        }

        payment.Status = PaymentStatus.Refunded;
        payment.PaidAt = null;

        await _unitOfWork.SaveChangesAsync();

        Log.Information("Webhook — payment record updated: {Order} → Refunded",
            payment.MerchantOrderId);
    }

    // ---------------------------------------------------------------
    // Domain: Booking Payment Success
    // ---------------------------------------------------------------
    private async Task HandleBookingPaymentSuccessAsync(Payment payment)
    {
        if (!payment.ServiceBookingId.HasValue)
        {
            Log.Warning("Webhook — booking payment has no ServiceBookingId: {PaymentId}", payment.Id);
            return;
        }

        var booking = await _unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician)
            .Include(x => x.ServiceRequest)
                .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(b => b.Id == payment.ServiceBookingId.Value);

        if (booking == null)
        {
            Log.Warning("Webhook — booking not found: {BookingId}", payment.ServiceBookingId);
            return;
        }

        // Update booking & counters
        booking.Status                                    = ServiceBookingStatus.Completed;
        booking.Technician.CompletedBookings             += 1;
        booking.ServiceRequest.Customer.CompletedBookings += 1;

        // Create payout for technician
        var payout = new Domain.Entities.Payments.Payout
        {
            TechnicianId = booking.TechnicianId,
            BookingId    = booking.Id,
            Amount       = payment.TechnicianAmount,
            Status       = PayoutStatus.Pending,
            CreatedAt    = DateTime.UtcNow
        };

        await _unitOfWork.Payouts.AddAsync(payout);

        // Notify customer
        await _notificationService.SendFullNotificationAsync(
            booking.ServiceRequest.Customer,
            NotificationType.BookingCompleted,
            SharedResourcesKeys.NotificationBookingCompletedTitle,
            SharedResourcesKeys.NotificationBookingCompletedBody
        );

        Log.Information("Webhook — payout created: {Amount:C} for technician {TechnicianId}",
            payment.TechnicianAmount, booking.TechnicianId);
    }

    // ---------------------------------------------------------------
    // Domain: Commission Payment Success
    // ---------------------------------------------------------------
    private async Task HandleCommissionPaymentSuccessAsync(Payment payment)
    {
        // Format: COMM-{guid} → parts[1..5] = guid
        var parts = payment.MerchantOrderId.Split('-');

        if (parts.Length < 6)
        {
            Log.Error("Webhook — invalid merchant order format: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        var technicianIdString = $"{parts[1]}-{parts[2]}-{parts[3]}-{parts[4]}-{parts[5]}";

        if (!Guid.TryParse(technicianIdString, out var technicianId))
        {
            Log.Error("Webhook — cannot extract technician ID from: {OrderRef}", payment.MerchantOrderId);
            return;
        }

        var commissions = await _unitOfWork.TechnicianCommissionsOwed.GetTableAsTracking()
            .Where(c => c.TechnicianId == technicianId && !c.IsPaid)
            .ToListAsync();

        if (!commissions.Any())
        {
            Log.Warning("Webhook — no unpaid commissions for technician {TechnicianId}", technicianId);
            return;
        }

        foreach (var commission in commissions)
        {
            commission.IsPaid = true;
            commission.PaidAt = DateTime.UtcNow;
        }

        Log.Information("Webhook — marked {Count} commissions as paid for technician {TechnicianId}",
            commissions.Count, technicianId);
    }

    // ---------------------------------------------------------------
    // Private Helpers
    // ---------------------------------------------------------------
    private async Task<string> GetOrCreateCustomerAsync(string name, string email, string phone)
    {
        var searchOptions = new CustomerSearchOptions { Query = $"email:'{email}'" };
        var searchService = new CustomerService();
        var existing      = await searchService.SearchAsync(searchOptions);

        if (existing.Data.Any())
            return existing.Data.First().Id;

        var createOptions = new CustomerCreateOptions { Name = name, Email = email, Phone = phone };
        var customer      = await searchService.CreateAsync(createOptions);
        return customer.Id;
    }

    private long ConvertToSmallestUnit(decimal amount)
        => (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
}