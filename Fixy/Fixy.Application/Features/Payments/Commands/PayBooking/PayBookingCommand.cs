using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.PayBooking;

public record PayBookingCommand(Guid BookingId) : IRequest<Result>;
