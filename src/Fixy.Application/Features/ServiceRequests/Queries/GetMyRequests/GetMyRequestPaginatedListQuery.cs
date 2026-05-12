using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public sealed record GetMyRequestPaginatedListQuery
    (
        int PageNumber,
        int PageSize,
        ServiceRequestOrdering OrderBy,
        string? Search
    )
    : IRequest<Result<PaginatedResult<GetMyRequestPaginatedListResponse>>>;