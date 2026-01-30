using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public record MarkBookingCompletedCommand(Guid BookingId) : IRequest<Result>;
