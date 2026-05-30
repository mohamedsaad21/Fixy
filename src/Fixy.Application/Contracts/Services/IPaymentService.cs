using Fixy.Application.Common.DTOs.Payment;

namespace Fixy.Application.Contracts.Services;

public interface IPaymentService
{
    Task<PaymentUrlResult> CreatePaymentUrlAsync(decimal amount, Guid referenceId, string customerName, string customerEmail, string customerPhone, string orderPrefix = "BK");
    Task<bool> VerifyWebhookSignature(string payload, string signature);
}
