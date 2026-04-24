using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestPaginaredList;

public sealed class GetServiceRequestPaginaredListQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetServiceRequestPaginaredListQuery, Result<PaginatedResult<Common.DTOs.GetServiceRequestListResponse>>>
{
    public async Task<Result<PaginatedResult<Common.DTOs.GetServiceRequestListResponse>>> Handle(GetServiceRequestPaginaredListQuery request, CancellationToken cancellationToken)
    {
        var serviceRequests = unitOfWork.ServiceRequests
            .GetTableNoTracking().Include(x => x.Customer).Include(x => x.ServiceCategories);
        var paginatedResponse = await serviceRequests.Select(x => x.ToServiceRequestListResponse()).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return paginatedResponse;
    }
}
