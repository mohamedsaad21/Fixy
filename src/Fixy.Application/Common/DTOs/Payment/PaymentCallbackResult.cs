namespace Fixy.Application.Common.DTOs.Payment;

public class PaymentCallbackResult
{
    public bool Success { get; set; }
    public string PaymentIntentId { get; set; }
    public string OrderReference { get; set; }
    public string Status { get; set; }            // succeeded, failed, pending
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; }
    public string ErrorMessage { get; set; }
}
