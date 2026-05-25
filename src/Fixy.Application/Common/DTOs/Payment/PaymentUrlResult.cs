namespace Fixy.Application.Common.DTOs.Payment;

public class PaymentUrlResult
{
    public bool Success { get; set; }
    public string ClientSecret { get; set; } 
    public string PaymentIntentId { get; set; }
    public string MerchantOrderId { get; set; }
    public string ErrorMessage { get; set; }
}
