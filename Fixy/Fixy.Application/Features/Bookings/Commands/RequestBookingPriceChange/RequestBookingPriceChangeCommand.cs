using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public record RequestBookingPriceChangeCommand(Guid BookingId, decimal NewProposedPrice) : IRequest<Result>;
