using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.PayBooking.Responses;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.PayBooking;

public sealed record PayBookingCommand
    (
        Guid BookingId,
        PaymentMethod PaymentMethod
    ) : IRequest<Result<PayBookingResponse>>;
