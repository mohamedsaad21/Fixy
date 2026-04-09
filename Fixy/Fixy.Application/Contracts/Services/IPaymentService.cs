using Fixy.Application.Common.DTOs.Payment;

namespace Fixy.Application.Contracts.Services;

public interface IPaymentService
{
    /// <summary>
    /// Create payment URL for customer
    /// Returns the full URL to redirect customer to
    /// </summary>
    Task<PaymentUrlResult> CreatePaymentUrlAsync(decimal amount, Guid referenceId, string customerName, string customerEmail, string customerPhone, string orderPrefix = "BK");
    Task<PaymentCallbackResult> ProcessCallbackAsync();
    Task<bool> VerifyWebhookSignature(string payload, string signature);
}
