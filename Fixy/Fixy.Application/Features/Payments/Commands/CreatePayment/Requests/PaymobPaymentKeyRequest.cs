using Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment.Requests;

public class PaymobPaymentKeyRequest
{
    public string AuthToken { get; set; }
    public int AmountCents { get; set; }
    public string OrderId { get; set; }
    public int IntegrationId { get; set; }
    public PaymobBillingData BillingData { get; set; }
}
