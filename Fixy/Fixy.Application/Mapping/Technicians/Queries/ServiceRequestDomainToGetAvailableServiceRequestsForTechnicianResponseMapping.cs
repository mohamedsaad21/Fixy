using Fixy.Application.Common.DTOs;
using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.Technicians.Queries.GetAvailableServiceRequestsForTechnician;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.Technicians.Queries;

public static class ServiceRequestDomainToGetAvailableServiceRequestsForTechnicianResponseMapping
{
    public static GetAvailableServiceRequestsForTechnicianResponse ToGetAvailableServiceRequestsForTechnicianResponse(this ServiceRequest serviceRequest, double distanceKm)
    {
        return new GetAvailableServiceRequestsForTechnicianResponse
        {
            Id = serviceRequest.Id,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime.ToEgyptTime(),
            Address = new AddressDto(
                serviceRequest.Address.Country, 
                serviceRequest.Address.City, 
                serviceRequest.Address.Area,
                serviceRequest.Address.Street,
                serviceRequest.Address.BuildingNumber,
                serviceRequest.Address.Latitude,
                serviceRequest.Address.Longitude
            ),
            Status = serviceRequest.Status.ToString(),
            ServiceCategories = serviceRequest.ServiceCategories.Select(c => c.NameEn).ToList(),
            ImageUrls = serviceRequest.ServiceRequestImages.Select(i => i.ImageUrl).ToList(),
            PriceOffersCount = serviceRequest.PriceOffers.Count,
            DistanceKm = Math.Round(distanceKm, 2)
        };
    }
}
