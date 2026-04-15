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

    // ─────────────────────────────────────────────
    // Only one session type now — wallet top-up
    // Covers: manual top-up, booking shortfall,
    //         technician debt clearance
    // ─────────────────────────────────────────────
    public async Task<PaymentUrlResult> CreateTopUpSessionAsync(decimal amount, string userId, string customerName, string customerEmail, string customerPhone)
    {
        try
        {
            var merchantOrderId =
                $"TOPUP-{userId}-{Guid.NewGuid().ToString("N")[..8]}";

            Log.Information(
                "Creating Stripe top-up session - " +
                "Order: {MerchantOrderId}, Amount: {Amount}",
                merchantOrderId, amount);

            var amountCents = (long)(amount * 100);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency    = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name        = "Fixy Wallet Top-Up",
                                Description = $"Order: {merchantOrderId}"
                            },
                            UnitAmount = amountCents
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{_stripeSettings.SuccessUrl}" +
                                    $"?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = _stripeSettings.CancelUrl,
                CustomerEmail = customerEmail,
                ClientReferenceId = merchantOrderId,
                Metadata = new Dictionary<string, string>
                {
                    { "merchant_order_id", merchantOrderId },
                    { "user_id",           userId          },
                    { "customer_name",     customerName    },
                    { "customer_phone",    customerPhone   }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            Log.Information(
                "Stripe top-up session created - SessionId: {SessionId}",
                session.Id);

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
            Log.Error(ex,
                "Stripe error creating top-up session - " +
                "Order: {MerchantOrderId}",
                $"TOPUP-{userId}");
            throw new Exception($"Stripe top-up failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Unexpected error creating top-up session - UserId: {UserId}",
                userId);
            throw;
        }
    }

    // ─────────────────────────────────────────────
    // Webhook — only handles TOPUP-* now
    // ─────────────────────────────────────────────
    public async Task<PaymentCallbackResult> ProcessCallbackAsync()
    {
        try
        {
            Log.Information("Processing Stripe webhook");

            string json;
            using (var reader = new StreamReader(
                _httpContextAccessor.HttpContext!.Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            var stripeSignature = _httpContextAccessor.HttpContext.Request
                .Headers["Stripe-Signature"].ToString();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                Log.Error(ex, "Invalid Stripe webhook signature");
                throw new Exception("Invalid webhook signature");
            }

            Log.Information(
                "Stripe event received - Type: {EventType}",
                stripeEvent.Type);

            // ── Completed ──────────────────────────
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                if (session is null)
                {
                    Log.Error("Failed to cast Stripe event data to Session");
                    return null!;
                }

                if (string.IsNullOrEmpty(session.ClientReferenceId))
                {
                    Log.Error(
                        "ClientReferenceId is null - SessionId: {SessionId}",
                        session.Id);
                    return null!;
                }

                Log.Information(
                    "Top-up successful - SessionId: {SessionId}, " +
                    "MerchantOrderId: {MerchantOrderId}, Amount: {Amount}",
                    session.Id,
                    session.ClientReferenceId,
                    session.AmountTotal / 100m);

                return new PaymentCallbackResult
                {
                    Success = true,
                    TransactionId = session.PaymentIntentId,
                    MerchantOrderId = session.ClientReferenceId,
                    Amount = session.AmountTotal.HasValue
                                        ? session.AmountTotal.Value / 100m
                                        : 0,
                    Provider = "Stripe",
                    Status = "completed",
                    Metadata = new Dictionary<string, object>
                    {
                        { "session_id",        session.Id                    },
                        { "payment_intent_id", session.PaymentIntentId ?? "" },
                        { "customer_email",    session.CustomerEmail    ?? "" }
                    }
                };
            }

            // ── Expired ────────────────────────────
            if (stripeEvent.Type == EventTypes.CheckoutSessionExpired)
            {
                var session = stripeEvent.Data.Object as Session;

                Log.Warning(
                    "Top-up session expired - SessionId: {SessionId}",
                    session!.Id);

                return new PaymentCallbackResult
                {
                    Success = false,
                    TransactionId = session.Id,
                    MerchantOrderId = session.ClientReferenceId,
                    Amount = session.AmountTotal.HasValue
                                        ? session.AmountTotal.Value / 100m
                                        : 0,
                    Provider = "Stripe",
                    Status = "expired"
                };
            }

            // ── Failed ─────────────────────────────
            if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                Log.Warning(
                    "Top-up payment failed - PaymentIntentId: {PaymentIntentId}",
                    paymentIntent!.Id);

                return new PaymentCallbackResult
                {
                    Success = false,
                    TransactionId = paymentIntent.Id,
                    MerchantOrderId = paymentIntent.Metadata
                                        .GetValueOrDefault("merchant_order_id"),
                    Amount = paymentIntent.Amount / 100m,
                    Provider = "Stripe",
                    Status = "failed"
                };
            }

            Log.Warning(
                "Unhandled Stripe event type: {EventType}",
                stripeEvent.Type);

            return null!;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing Stripe webhook");
            throw;
        }
    }

    // ─────────────────────────────────────────────
    // Signature verification
    // ─────────────────────────────────────────────
    public async Task<bool> VerifyWebhookSignature(string payload, string signature)
    {
        try
        {
            EventUtility.ConstructEvent(
                payload,
                signature,
                _stripeSettings.WebhookSecret);
            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}
