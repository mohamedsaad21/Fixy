using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestPaginaredList;

public sealed record GetServiceRequestPaginaredListQuery
    (
        int PageSize,
        int PageNumber,
        BookingOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<Common.DTOs.GetServiceRequestListResponse>>>;
