using Fixy.Domain.Enums;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public string PaymentUrl { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
