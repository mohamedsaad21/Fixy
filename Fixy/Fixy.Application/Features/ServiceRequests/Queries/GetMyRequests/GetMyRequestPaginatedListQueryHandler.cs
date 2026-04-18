using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.ServiceRequests.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public sealed class GetMyRequestPaginatedListQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetMyRequestPaginatedListQuery, Result<PaginatedResult<GetMyRequestPaginatedListResponse>>>
{
    public async Task<Result<PaginatedResult<GetMyRequestPaginatedListResponse>>> Handle(GetMyRequestPaginatedListQuery request, CancellationToken cancellationToken)
    {
        var currentCustomerId = currentUserService.GetCurrentUserId(); 
        var myServiceRequests = unitOfWork.ServiceRequests.GetTableNoTracking().Where(x => x.CustomerId == currentCustomerId)
            .Include(x => x.Customer)
            .Include(x => x.ServiceCategories).AsQueryable();

        myServiceRequests = request.OrderBy switch
        {
            ServiceRequestOrdering.Description => myServiceRequests.OrderBy(x => x.Description),
            ServiceRequestOrdering.ScheduledDateTime => myServiceRequests.OrderBy(x => x.ScheduledDateTime),
            ServiceRequestOrdering.CreatedAt => myServiceRequests.OrderBy(x => x.CreatedAt),
            _ => myServiceRequests
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            myServiceRequests = myServiceRequests.Where(x => x.Description.Contains(request.Search));
        }

        var PaginatedList = await myServiceRequests.Select(x => x.ToGetMyRequestPaginatedListResponse()).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return PaginatedList;
    }
}
