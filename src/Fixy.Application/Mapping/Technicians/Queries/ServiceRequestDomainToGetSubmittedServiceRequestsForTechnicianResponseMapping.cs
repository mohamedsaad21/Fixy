using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.Technicians.Queries.GetSubmittedServiceRequestsForTechnician;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.Technicians.Queries;

public static class ServiceRequestDomainToGetSubmittedServiceRequestsForTechnicianResponseMapping
{
    public static GetSubmittedServiceRequestsForTechnicianResponse ToGetSubmittedServiceRequestsForTechnicianResponse(this ServiceRequest serviceRequest, Guid TechnicianId)
    {
        return new GetSubmittedServiceRequestsForTechnicianResponse
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
            ServiceCategories = serviceRequest.ServiceCategories.Select(c => c.Localize(c.NameAr, c.NameEn)).ToList(),
            ImageUrls = serviceRequest.ServiceRequestImages.Select(i => i.ImageUrl).ToList(),
            PriceOffer = serviceRequest.PriceOffers.FirstOrDefault(x => x.TechnicianId == TechnicianId).Price,
            PriceOffersCount = serviceRequest.PriceOffers.Count,
        };
    }
}
