using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetTechnicians;

public sealed record GetTechniciansQuery
    (
        int PageNumber,
        int PageSize,
        TechnicianOrdering OrderBy,
        SortOrderOptions SortOrder,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetTechniciansResponse>>>;