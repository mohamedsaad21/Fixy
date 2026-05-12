using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;

public sealed record GetBookingsForTechnicianQuery
    (
        int PageSize,
        int PageNumber,
        BookingOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetBookingsForTechnicianResponse>>>;