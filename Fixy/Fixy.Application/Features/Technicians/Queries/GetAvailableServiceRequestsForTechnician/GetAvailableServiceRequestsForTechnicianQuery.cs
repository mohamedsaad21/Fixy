using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetAvailableServiceRequestsForTechnician;

public sealed record GetAvailableServiceRequestsForTechnicianQuery
    (
        int PageNumber,
        int PageSize,
        BookingOrdering OrderBy,
        string? Search,
        double RadiusKm = 20.0
    ) : IRequest<Result<PaginatedResult<GetAvailableServiceRequestsForTechnicianResponse>>>;
