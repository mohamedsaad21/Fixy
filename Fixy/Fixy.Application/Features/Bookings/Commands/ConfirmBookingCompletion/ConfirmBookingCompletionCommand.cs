using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;

public sealed record ConfirmBookingCompletionCommand(Guid BookingId) : IRequest<Result>;
