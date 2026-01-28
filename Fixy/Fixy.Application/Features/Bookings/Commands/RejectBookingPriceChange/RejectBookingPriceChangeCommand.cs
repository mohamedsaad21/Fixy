using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public record RejectBookingPriceChangeCommand(Guid BookingId) : IRequest<Result>;
