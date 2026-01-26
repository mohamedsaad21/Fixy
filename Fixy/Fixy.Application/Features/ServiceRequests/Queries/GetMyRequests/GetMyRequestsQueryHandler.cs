using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyRequestsQueryHandler(IServiceRequestRepository serviceRequestRepository, ICurrentUserService currentUserService)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetMyRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentCustomerId = _currentUserService.GetCurrentUserId(); 
        var myServiceRequests = await _serviceRequestRepository.GetTableNoTracking().Where(x => x.CustomerId == currentCustomerId)
            .Include(x => x.Customer)
            .Include(x => x.ServiceCategories).ToListAsync();
        var result = myServiceRequests.Select(x => x.ToServiceRequestListDto()).ToList();
        return result;
    }
}
