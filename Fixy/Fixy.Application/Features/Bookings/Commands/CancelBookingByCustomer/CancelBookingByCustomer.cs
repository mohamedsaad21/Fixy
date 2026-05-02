using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;

public sealed record CancelBookingByCustomerCommand
    (
        Guid BookingId,
        CustomerCancellationReason Reason,
        string? Note,
        bool ReopenServiceRequest
    ) : IRequest<Result>;
