using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;

public sealed record CancelBookingByTechnicianCommand
    (
        Guid BookingId,
        TechnicianCancellationReason Reason,
        string? Notes
    ) : IRequest<Result>;
