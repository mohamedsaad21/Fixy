using Fixy.Application.Common.DTOs;
using Stripe;

namespace Fixy.Application.Abstracts;

public interface IStripePaymentService
{
    Task<PaymentIntentResultDto> CreatePaymentIntentAsync(Guid bookingId);
    Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId);
    Task<bool> CancelPaymentAsync(string paymentIntentId);
}
