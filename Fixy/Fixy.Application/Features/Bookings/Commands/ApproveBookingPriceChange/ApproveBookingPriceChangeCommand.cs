using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public sealed record ApproveBookingPriceChangeCommand(Guid BookingId) : IRequest<Result>;
