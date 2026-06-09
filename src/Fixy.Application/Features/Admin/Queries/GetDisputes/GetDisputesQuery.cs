using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetDisputes;

public class GetDisputesQuery : IRequest<Result<PaginatedResult<DisputeDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DisputeStatus? Status { get; set; }
}
