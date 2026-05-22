using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetSubmittedServiceRequestsForTechnician;

public sealed record GetSubmittedServiceRequestsForTechnicianQuery
    (
        int PageNumber,
        int PageSize,
        BookingOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetSubmittedServiceRequestsForTechnicianResponse>>>;
