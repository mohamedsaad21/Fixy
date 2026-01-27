using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;

public class GetTechnicianAvailableRequestsQueryHandler : IRequestHandler<GetTechnicianAvailableRequestsQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IServiceRequestRepository _serviceRequestRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITechnicianLocationRepository _technicianLocationRepository;
    private const int DefaultMaxDistanceKm = 25;

    public GetTechnicianAvailableRequestsQueryHandler(IServiceRequestRepository serviceRequestRepository, 
        ICurrentUserService currentUserService,
        ITechnicianLocationRepository technicianLocationRepository)
    {
        _serviceRequestRepository = serviceRequestRepository;
        _currentUserService = currentUserService;
        _technicianLocationRepository = technicianLocationRepository;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetTechnicianAvailableRequestsQuery request, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var location = await _technicianLocationRepository.GetTableNoTracking().Where(x => x.TechnicianId == currentTechnician.Id).FirstOrDefaultAsync();
        // if location not exist 
        if (location == null)
            return Errors.LocationNotUpdated;
        
        var availableServiceRequests = await _serviceRequestRepository.GetTableNoTracking()            
            .Where(x => x.Status == ServiceRequestStatus.Pending).Include(x => x.ServiceCategories).Include(x => x.Customer)
            .Where(x => x.ServiceCategories.Any(x => x.Id == technician.ServiceCategoryId))
            .ToListAsync();
        var FilteredServiceRequestsByLocation = availableServiceRequests
            .Where(x => GeoDistance.CalculateKm(location.Latitude, location.Longitude, x.Address.Latitude, x.Address.Longitude) 
            <= DefaultMaxDistanceKm).Select(x => x.ToServiceRequestListDto()).ToList();
        return FilteredServiceRequestsByLocation;
    }
}
