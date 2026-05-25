using Fixy.Application.Common.DTOs.Payment;
using Fixy.Application.Contracts.Services;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripeService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StripeService(StripeSettings stripeSettings, IHttpContextAccessor httpContextAccessor)
    {
        _stripeSettings = stripeSettings;
        _httpContextAccessor = httpContextAccessor;

        StripeConfiguration.ApiKey = _stripeSettings.Secretkey;
    }

    // ---------------------------------------------------------------
    // CreatePaymentUrlAsync
    // Returns ClientSecret to Angular instead of a redirect URL
    // ---------------------------------------------------------------
    public async Task<PaymentUrlResult> CreatePaymentUrlAsync(
        decimal amount,
        Guid referenceId,
        string customerName,
        string customerEmail,
        string customerPhone,
        string orderPrefix = "BK")
    {
        try
        {
            var orderReference = $"{orderPrefix}-{referenceId.ToString().ToUpper()}";

            // Step 1: Get or create Stripe Customer
            var customerId = await GetOrCreateCustomerAsync(
                customerName,
                customerEmail,
                customerPhone
            );

            // Step 2: Create PaymentIntent
            var options = new PaymentIntentCreateOptions
            {
                Amount = ConvertToSmallestUnit(amount),   // e.g. 10.00 → 1000
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
                Description = $"Order {orderReference} for {customerName}",
                ReceiptEmail = customerEmail,
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            Log.Information(
                "PaymentIntent created: {Id} for order {Order}",
                paymentIntent.Id, orderReference);

            return new PaymentUrlResult
            {
                Success = true,
                ClientSecret = paymentIntent.ClientSecret,  // → sent to Angular
                PaymentIntentId = paymentIntent.Id,
                MerchantOrderId = orderReference,
            };
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Stripe error creating PaymentIntent");
            return new PaymentUrlResult
            {
                Success = false,
                ErrorMessage = ex.StripeError?.Message ?? ex.Message
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error creating PaymentIntent");
            return new PaymentUrlResult
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred."
            };
        }
    }

    // ---------------------------------------------------------------
    // ProcessCallbackAsync
    // Called after Angular confirms payment — verify status server-side
    // ---------------------------------------------------------------
    public async Task<PaymentCallbackResult> ProcessCallbackAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Angular sends paymentIntentId in request body
            var request = httpContext.Request;
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            var callbackData = System.Text.Json.JsonSerializer
                .Deserialize<CallbackRequest>(body,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (string.IsNullOrEmpty(callbackData?.PaymentIntentId))
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage = "PaymentIntentId is required."
                };
            }

            // Retrieve PaymentIntent from Stripe to verify
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(callbackData.PaymentIntentId);

            var orderReference = paymentIntent.Metadata.GetValueOrDefault("order_reference");
            var amount = paymentIntent.Amount / 100m;
            var customerEmail = paymentIntent.Metadata.GetValueOrDefault("customer_email");

            if (paymentIntent.Status == "succeeded")
            {
                Log.Information(
                    "Payment succeeded for order {Order}", orderReference);

                return new PaymentCallbackResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    OrderReference = orderReference,
                    Status = "succeeded",
                    Amount = amount,
                    CustomerEmail = customerEmail,
                };
            }

            return new PaymentCallbackResult
            {
                Success = false,
                PaymentIntentId = paymentIntent.Id,
                OrderReference = orderReference,
                Status = paymentIntent.Status,
                Amount = amount,
                ErrorMessage = $"Payment status: {paymentIntent.Status}"
            };
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Stripe error processing callback");
            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = ex.StripeError?.Message ?? ex.Message
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error processing callback");
            return new PaymentCallbackResult
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred."
            };
        }
    }

    // ---------------------------------------------------------------
    // VerifyWebhookSignature
    // Called by Stripe webhook endpoint
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

            // Handle webhook events
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentSucceeded(succeededIntent);
                    break;

                case "payment_intent.payment_failed":
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    await HandlePaymentFailed(failedIntent);
                    break;

                case "charge.refunded":
                    var charge = stripeEvent.Data.Object as Charge;
                    await HandleRefund(charge);
                    break;
            }

            return true;
        }
        catch (StripeException ex)
        {
            Log.Error(ex, "Webhook signature verification failed");
            return false;
        }
    }

    // ---------------------------------------------------------------
    // Private Helpers
    // ---------------------------------------------------------------
    private async Task<string> GetOrCreateCustomerAsync(
        string name, string email, string phone)
    {
        var searchOptions = new CustomerSearchOptions
        {
            Query = $"email:'{email}'"
        };
        var searchService = new CustomerService();
        var existing = await searchService.SearchAsync(searchOptions);

        if (existing.Data.Any())
            return existing.Data.First().Id;

        var createOptions = new CustomerCreateOptions
        {
            Name = name,
            Email = email,
            Phone = phone,
        };

        var customer = await searchService.CreateAsync(createOptions);
        return customer.Id;
    }

    private long ConvertToSmallestUnit(decimal amount)
    {
        // Converts dollars/pounds/euros to cents/pence
        return (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);
    }

    private Task HandlePaymentSucceeded(PaymentIntent intent)
    {
        Log.Information(
            "Payment succeeded via webhook: {Id}, Order: {Order}",
            intent.Id,
            intent.Metadata.GetValueOrDefault("order_reference"));

        // TODO: Update your database order status here
        return Task.CompletedTask;
    }

    private Task HandlePaymentFailed(PaymentIntent intent)
    {
        Log.Warning(
            "Payment failed via webhook: {Id}, Order: {Order}",
            intent.Id,
            intent.Metadata.GetValueOrDefault("order_reference"));

        // TODO: Update your database order status here
        return Task.CompletedTask;
    }

    private Task HandleRefund(Charge charge)
    {
        Log.Information("Charge refunded: {Id}", charge.Id);

        // TODO: Handle refund logic here
        return Task.CompletedTask;
    }
}
