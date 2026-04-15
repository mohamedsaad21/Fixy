using Fixy.Domain.Enums;

namespace Fixy.Application.Features.Payments.Commands.PayBooking.Responses;

public class PayBookingResponse
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
