using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public sealed record RequestBookingPriceChangeCommand(Guid BookingId, decimal NewProposedPrice) : IRequest<Result>;
