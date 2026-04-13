using Fixy.Application.Common.DTOs.Payment;
using Fixy.Application.Contracts.Services;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Serilog;
using Stripe;
using Stripe.Checkout;

namespace Fixy.Infrastructure.Services;

public class StripeService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public StripeService(StripeSettings stripeSettings, IHttpContextAccessor httpContextAccessor)
    {
        _stripeSettings = stripeSettings;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaymentUrlResult> CreatePaymentUrlAsync(decimal amount, Guid referenceId, string customerName, string customerEmail, string customerPhone, string orderPrefix = "BK")
    {
        try
        {
            var merchantOrderId = $"{orderPrefix}-{referenceId}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            Log.Information($"Creating Stripe payment session - Order: {merchantOrderId}, Amount: {amount:C}");

            // Convert amount to cents (Stripe uses smallest currency unit)
            var amountCents = (long)(amount * 100);

            // Create Stripe Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "egp",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = orderPrefix == "BK"
                                        ? "Service Booking Payment"
                                        : "Commission Payment",
                                    Description = $"Order: {merchantOrderId}"
                                },
                                UnitAmount = amountCents
                            },
                            Quantity = 1
                        }
                    },
                Mode = "payment",
                SuccessUrl = $"{_stripeSettings.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = _stripeSettings.CancelUrl,
                CustomerEmail = customerEmail,
                ClientReferenceId = merchantOrderId,
                Metadata = new Dictionary<string, string>
                    {
                        { "merchant_order_id", merchantOrderId },
                        { "order_prefix", orderPrefix },
                        { "reference_id", referenceId.ToString() },
                        { "customer_name", customerName },
                        { "customer_phone", customerPhone }
                    }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            Log.Information($"Stripe session created - SessionId: {session.Id}");

            return new PaymentUrlResult
            {
                PaymentUrl = session.Url,
                StripeSessionId = session.Id,
                MerchantOrderId = merchantOrderId,
                Provider = "Stripe"
            };
        }
        catch (StripeException ex)
        {
            Log.Error(ex, $"Stripe error creating payment session");
            throw new Exception($"Stripe payment failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error creating Stripe payment session");
            throw;
        }
    }

    public async Task<PaymentCallbackResult> ProcessCallbackAsync()
    {
        try
        {
            Log.Information("Processing Stripe webhook");

            // Read request body
            string json;
            using (var reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            // Verify webhook signature
            var stripeSignature = _httpContextAccessor.HttpContext.Request.Headers["Stripe-Signature"].ToString();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );
            }
            catch (StripeException ex)
            {
                Log.Error(ex, "Invalid Stripe webhook signature");
                throw new Exception("Invalid webhook signature");
            }

            Log.Information($"Stripe event type: {stripeEvent.Type}");

            // Handle different event types
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                if (session == null)
                {
                    Log.Error("Failed to cast event data to Session");
                    return null;
                }

                if (string.IsNullOrEmpty(session.ClientReferenceId))
                {
                    Log.Error("ClientReferenceId is null - SessionId: {SessionId}", session.Id);
                    return null;
                }

                Log.Information("Payment successful - SessionId: {SessionId}, MerchantOrderId: {MerchantOrderId}, Amount: {Amount}",
                    session.Id,
                    session.ClientReferenceId,
                    session.AmountTotal / 100m);

                return new PaymentCallbackResult
                {
                    Success = true,
                    TransactionId = session.PaymentIntentId,
                    MerchantOrderId = session.ClientReferenceId,
                    Amount = session.AmountTotal.HasValue ? session.AmountTotal.Value / 100m : 0,
                    Provider = "Stripe",
                    Status = "completed",
                    Metadata = new Dictionary<string, object>
        {
            { "session_id", session.Id },
            { "payment_intent_id", session.PaymentIntentId ?? "" },
            { "customer_email", session.CustomerEmail ?? "" }
        }
                };
            }
            else if (stripeEvent.Type == EventTypes.CheckoutSessionExpired)
            {
                var session = stripeEvent.Data.Object as Session;

                Log.Warning($"Payment expired - SessionId: {session.Id}");

                return new PaymentCallbackResult
                {
                    Success = false,
                    TransactionId = session.Id,
                    MerchantOrderId = session.ClientReferenceId,
                    Amount = session.AmountTotal.Value / 100m,
                    Provider = "Stripe",
                    Status = "expired"
                };
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                Log.Warning($"Payment failed - PaymentIntentId: {paymentIntent.Id}");

                return new PaymentCallbackResult
                {
                    Success = false,
                    TransactionId = paymentIntent.Id,
                    MerchantOrderId = paymentIntent.Metadata.GetValueOrDefault("merchant_order_id"),
                    Amount = paymentIntent.Amount / 100m,
                    Provider = "Stripe",
                    Status = "failed"
                };
            }

            Log.Warning($"Unhandled Stripe event type: {stripeEvent.Type}");
            return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing Stripe webhook");
            throw;
        }
    }

    public async Task<bool> VerifyWebhookSignature(string payload, string signature)
    {
        try
        {
            EventUtility.ConstructEvent(payload, signature, _stripeSettings.WebhookSecret);
            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}
