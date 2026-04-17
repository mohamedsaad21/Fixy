using Fixy.Application.Common.DTOs.Payment;

namespace Fixy.Application.Contracts.Services;

public interface IPaymentService
{
    // Stripe Payments
    Task<PaymentUrlResult> CreateSessionAsync(decimal amount, string userId, string customerName, string customerEmail, string customerPhone);
    Task<PaymentCallbackResult> ProcessCallbackAsync();
    Task<bool> VerifyWebhookSignature(string payload, string signature);
    // Stripe Connect Payouts
    Task<string> CreateConnectAccountAsync(string technicianId, string email, string firstName, string lastName);
    Task<string> CreateOnboardingLinkAsync(string stripeAccountId);
    Task<StripeTransferResult> TransferToTechnicianAsync(string stripeAccountId, decimal amount, string payoutId);
    Task<bool> IsAccountOnboardedAsync(string stripeAccountId);
}
