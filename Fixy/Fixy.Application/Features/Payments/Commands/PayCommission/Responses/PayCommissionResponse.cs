namespace Fixy.Application.Features.Payments.Commands.PayCommission.Responses;

public class PayCommissionResponse
{
    public string PaymentUrl { get; set; }
    public decimal TotalAmount { get; set; }
    public int CommissionCount { get; set; }
    public string MerchantOrderId { get; set; }
}
