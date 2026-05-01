using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Technicians.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Enums;
using Fixy.Domain.Helpers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetAvailableServiceRequestsForTechnician;

public sealed class GetAvailableServiceRequestsForTechnicianQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetAvailableServiceRequestsForTechnicianQuery, Result<PaginatedResult<GetAvailableServiceRequestsForTechnicianResponse>>>
{
    public async Task<Result<PaginatedResult<GetAvailableServiceRequestsForTechnicianResponse>>> Handle(GetAvailableServiceRequestsForTechnicianQuery request, CancellationToken cancellationToken)
    {
        var technicianId = currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableNoTracking().Include(t => t.TechnicianLocation)
            .FirstOrDefaultAsync(x => x.Id == technicianId, cancellationToken);

        if (technician == null)
            return Errors.TechnicianNotFound;

        if (technician.TechnicianLocation == null)
            return Errors.TechnicianLocationNotSet;

        var techLat = technician.TechnicianLocation.Latitude;
        var techLon = technician.TechnicianLocation.Longitude;
        var techCategoryId = technician.ServiceCategoryId;

        // Filter by category + pending status at the DB level
        // This reduces the rows before we pull coordinates into memory
        var filteredRequests = await unitOfWork.ServiceRequests
            .GetTableNoTracking()
            .Where(sr =>
                sr.Status == ServiceRequestStatus.Pending &&
                sr.ServiceRequestCategories.Any(src => src.CategoryId == techCategoryId) &&
                sr.BlockedServiceRequests.Any(bsr => bsr.TechnicianId == technician.Id && bsr.ServiceRequestId == sr.Id) &&
                !sr.PriceOffers.Any(po => po.TechnicianId == technicianId))
            .Include(sr => sr.ServiceCategories)
            .Include(sr => sr.ServiceRequestImages)
            .Include(sr => sr.PriceOffers).ToListAsync();


        var nearbyRequests = filteredRequests
         .Select(sr => new
         {
             Request = sr,
             DistanceKm = HaversineDistance.CalculateDistance(techLat, techLon, sr.Address.Latitude, sr.Address.Longitude)
         })
         .Where(x => x.DistanceKm <= request.RadiusKm)
         .OrderBy(x => x.DistanceKm)
         .ToList();

        var totalCount = nearbyRequests.Count;
        var pagedItems = nearbyRequests.Select(x => x.Request.ToGetAvailableServiceRequestsForTechnicianResponse(x.DistanceKm)).ToList();

        var paginatedResult = PaginatedResult<GetAvailableServiceRequestsForTechnicianResponse>
            .Create(pagedItems, totalCount, request.PageNumber, request.PageSize);

        return paginatedResult;
    }
}
