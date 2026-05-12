namespace Fixy.Application.Features.Payments.Commands.CreatePayment.Requests;

public class PaymobOrderRequest
{
    public string AuthToken { get; set; }
    public int AmountCents { get; set; }
    public string Currency { get; set; }
    public string MerchantOrderId { get; set; }
}
