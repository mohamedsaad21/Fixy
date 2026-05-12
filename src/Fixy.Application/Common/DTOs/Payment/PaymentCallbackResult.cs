namespace Fixy.Application.Common.DTOs.Payment;

public class PaymentCallbackResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string MerchantOrderId { get; set; }
    public decimal Amount { get; set; }
    public string Provider { get; set; }
    public string Status { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
