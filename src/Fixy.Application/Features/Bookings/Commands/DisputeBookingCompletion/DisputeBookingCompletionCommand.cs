using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.DisputeBookingCompletion;

public sealed record DisputeBookingCompletionCommand(Guid BookingId, string Reason, string? DesiredResolution) : IRequest<Result>;
