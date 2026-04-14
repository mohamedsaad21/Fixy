using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.StartBooking;

public sealed record StartBookingCommand(Guid BookingId) : IRequest<Result>;