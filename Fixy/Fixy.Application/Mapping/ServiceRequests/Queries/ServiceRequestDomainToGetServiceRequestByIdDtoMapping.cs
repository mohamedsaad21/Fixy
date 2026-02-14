using Fixy.Application.Common.DTOs;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Application.Mapping.PriceOffers.Queries;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests.Queries;

public static class ServiceRequestDomainToGetServiceRequestByIdDtoMapping
{
    public static GetServiceRequestByIdDto ToServiceRequestByIdDto(this ServiceRequest serviceRequest)
    {
        return new GetServiceRequestByIdDto(
            serviceRequest.Id,
            serviceRequest.Customer.UserName,
            serviceRequest.Description,
            serviceRequest.ScheduledDateTime,
            serviceRequest.ServiceCategories.Select(x => x.Name).ToList(),
            new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            serviceRequest.Status,
            serviceRequest.PriceOffers
            .Select(x => x.ToPriceOfferDto(serviceRequest)).OrderByDescending(x => x.AverageRating).ThenBy(x => x.DistanceKm)
            .ThenBy(x => x.Price).ToList()
            );
    }
}
