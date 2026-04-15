using Fixy.Application.Common.DTOs.Payment;

namespace Fixy.Application.Contracts.Services;

public interface IPaymentService
{
    Task<PaymentUrlResult> CreateTopUpSessionAsync(
        decimal amount,
        string userId,
        string customerName,
        string customerEmail,
        string customerPhone);

    Task<PaymentCallbackResult> ProcessCallbackAsync();
    Task<bool> VerifyWebhookSignature(string payload, string signature);
}
