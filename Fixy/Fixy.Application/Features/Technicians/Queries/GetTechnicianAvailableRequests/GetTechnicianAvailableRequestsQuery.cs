using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;

public sealed record GetTechnicianAvailableRequestsQuery
    (
        int PageSize,
        int PageNumber,
        BookingOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetServiceRequestListDto>>>;
