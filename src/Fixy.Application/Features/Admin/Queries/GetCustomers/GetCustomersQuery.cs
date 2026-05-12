using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetCustomers;

public sealed record GetCustomersQuery
    (
        int PageNumber,
        int PageSize,
        CustomerOrdering OrderBy,
        string? Search
    ) : IRequest<Result<PaginatedResult<GetCustomersResponse>>>;