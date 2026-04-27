using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetBookings;

public sealed record GetBookingsQuery
    (
        int PageNumber,
        int PageSize,
        BookingOrdering OrderBy,
        SortOrderOptions SortOrder,
        string? Search,
        ServiceBookingStatus? Status,
        DateTime? FromDate,
        DateTime? ToDate
    ) : IRequest<Result<PaginatedResult<GetBookingsResponse>>>;