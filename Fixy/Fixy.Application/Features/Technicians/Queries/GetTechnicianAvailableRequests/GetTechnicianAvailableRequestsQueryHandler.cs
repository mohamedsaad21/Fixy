using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.ServiceRequests;
using Fixy.Application.Wrappers;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using Fixy.Domain.SP.TechnicianAvailableRequests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;

public sealed class GetTechnicianAvailableRequestsQueryHandler : IRequestHandler<GetTechnicianAvailableRequestsQuery, Result<List<ServiceRequestSpResult>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public GetTechnicianAvailableRequestsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<ServiceRequestSpResult>>> Handle(GetTechnicianAvailableRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var location = await _unitOfWork.TechnicianLocations.GetTableNoTracking().Where(x => x.TechnicianId == currentTechnician.Id).FirstOrDefaultAsync();
        // if location not exist 
        if (location == null)
            return Errors.LocationNotUpdated;

        var paginatedResponse = await _unitOfWork.ServiceRequestReadRepository
            .GetTechnicianAvailableServiceRequest(request.PageNumber, request.PageSize, technician.Id, location.Latitude, location.Longitude, technician.ServiceCategoryId);
        return paginatedResponse;
    }
}
