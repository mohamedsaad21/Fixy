using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;

public sealed class GetTechnicianAvailableRequestsQueryHandler : IRequestHandler<GetTechnicianAvailableRequestsQuery, Result<List<GetServiceRequestListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private const int DefaultMaxDistanceKm = 25;

    public GetTechnicianAvailableRequestsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<GetServiceRequestListDto>>> Handle(GetTechnicianAvailableRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var location = await _unitOfWork.TechnicianLocations.GetTableNoTracking().Where(x => x.TechnicianId == currentTechnician.Id).FirstOrDefaultAsync();
        // if location not exist 
        if (location == null)
            return Errors.LocationNotUpdated;
        
        var availableServiceRequests = await _unitOfWork.ServiceRequests.GetTableNoTracking()            
            .Where(x => x.Status == ServiceRequestStatus.Pending).Include(x => x.ServiceCategories).Include(x => x.Customer)
            .Where(x => x.ServiceCategories.Any(x => x.Id == technician.ServiceCategoryId))
            .ToListAsync();
        var FilteredServiceRequestsByLocation = availableServiceRequests
            .Where(x => GeoDistance.CalculateKm(location.Latitude, location.Longitude, x.Address.Latitude, x.Address.Longitude) 
            <= DefaultMaxDistanceKm).Select(x => x.ToServiceRequestListDto()).ToList();
        return FilteredServiceRequestsByLocation;
    }
}
