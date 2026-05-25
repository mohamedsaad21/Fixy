namespace Fixy.Application.Features.Payments.Commands.PayCommission.Responses;

public class PayCommissionResponse
{
    // Stripe Elements fields (replaces PaymentUrl + MerchantOrderId)
    public string ClientSecret { get; set; }   // → Angular mounts Elements with this
    public string PaymentIntentId { get; set; }   // → Angular sends back on callback
    public string OrderReference { get; set; }   // → COMM-{technicianId}

    public decimal TotalAmount { get; set; }
    public int CommissionCount { get; set; }
}