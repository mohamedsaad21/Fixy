using Fixy.Domain.Enums;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }

    // Stripe Elements fields (replaces PaymentUrl)
    public string? ClientSecret { get; set; }   // → Angular mounts Elements with this
    public string? PaymentIntentId { get; set; }  // → Angular sends back on callback
    public string? OrderReference { get; set; }  // → BK-{BookingId}
}