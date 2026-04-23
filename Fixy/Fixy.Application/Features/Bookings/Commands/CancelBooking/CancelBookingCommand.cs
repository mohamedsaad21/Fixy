using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : IRequest<Result>;
