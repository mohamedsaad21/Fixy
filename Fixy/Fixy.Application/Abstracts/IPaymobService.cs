using Fixy.Application.Common.DTOs.Payment;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Abstracts;

public interface IPaymobService
{
    /// <summary>
    /// Get authentication token from Paymob
    /// </summary>
    Task<string> GetAuthTokenAsync();

    /// <summary>
    /// Create payment URL for customer
    /// Returns the full URL to redirect customer to
    /// </summary>
    Task<PaymentUrlResult> CreatePaymentUrlAsync(decimal amount, Guid referenceId, string customerName, string customerEmail, string customerPhone, string orderPrefix = "BK");

    /// <summary>
    /// Verify HMAC signature from Paymob callback
    /// </summary>
    bool VerifyHmac(IQueryCollection query, string receivedHmac);
}
