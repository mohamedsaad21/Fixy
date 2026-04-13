using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;

public sealed record GetBookingsForCustomerQuery
    (
        int PageSize,
        int PageNumber,
        BookingOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetBookingsForCustomerResponse>>>;