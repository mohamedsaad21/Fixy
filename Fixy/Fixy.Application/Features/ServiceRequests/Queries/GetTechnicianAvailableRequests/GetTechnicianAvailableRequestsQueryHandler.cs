using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetTechnicianAvailableRequests;

public class GetTechnicianAvailableRequestsQueryHandler : IRequestHandler<GetTechnicianAvailableRequestsQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTechnicianAvailableRequestsQueryHandler(IServiceRequestRepository serviceRequestRepository, ICurrentUserService currentUserService)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetTechnicianAvailableRequestsQuery request, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        var currentTechnician = await _currentUserService.GetCurrentUserAsync() as Technician;
        var availableServiceRequests = await _serviceRequestRepository.GetTableNoTracking()            
            .Where(x => x.Status == ServiceRequestStatus.Pending).Include(x => x.ServiceCategories).Include(x => x.Customer)
            .Where(x => x.ServiceCategories.Any(x => x.Id == currentTechnician.ServiceCategoryId)).ToListAsync();
        var result = availableServiceRequests.Select(x => x.ToServiceRequestListDto()).ToList();
        return result;
    }
}
