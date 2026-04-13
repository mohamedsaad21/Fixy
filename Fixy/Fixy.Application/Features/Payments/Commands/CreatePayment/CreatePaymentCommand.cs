using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.CreatePayment.Responses;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand
    (
        Guid BookingId,
        Guid CustomerId,
        int PaymentMethod,
        string CustomerName,
        string CustomerEmail,
        string CustomerPhone
    ) : IRequest<Result<CreatePaymentResponse>>;