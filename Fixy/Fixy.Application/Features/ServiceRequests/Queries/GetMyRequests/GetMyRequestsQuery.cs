using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public sealed record GetMyRequestsQuery
    (
        int pageNumber,
        int pageSize,
        ServiceRequestOrdering OrderBy,
        string? Search
    )
    : IRequest<Result<PaginatedResult<GetServiceRequestListDto>>>;