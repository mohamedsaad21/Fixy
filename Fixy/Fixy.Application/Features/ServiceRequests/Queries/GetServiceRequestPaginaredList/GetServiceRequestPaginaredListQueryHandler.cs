using Fixy.Application.Bases;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestPaginaredList;

public sealed class GetServiceRequestPaginaredListQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer) : IRequestHandler<GetServiceRequestPaginaredListQuery, Result<PaginatedResult<Common.DTOs.GetServiceRequestListResponse>>>
{
    public async Task<Result<PaginatedResult<Common.DTOs.GetServiceRequestListResponse>>> Handle(GetServiceRequestPaginaredListQuery request, CancellationToken cancellationToken)
    {
        var serviceRequests = unitOfWork.ServiceRequests
            .GetTableNoTracking().Include(x => x.Customer).Include(x => x.ServiceCategories);
        var paginatedResponse = await serviceRequests.Select(x => x.ToServiceRequestListResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return paginatedResponse;
    }
}
