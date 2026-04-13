namespace Fixy.Application.Common.DTOs.Payment;

public class PaymentUrlResult
{
    public string PaymentUrl { get; set; }
    public string StripeSessionId { get; set; } 
    public string MerchantOrderId { get; set; }
    public int PaymobOrderId { get; set; }
    public string Provider {  get; set; }
}
