using Fixy.Application.Common.DTOs;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests;

public static class RequestDomainListToRequestListMapping
{
    public static GetServiceRequestListDto ToServiceRequestListDto(this ServiceRequest serviceRequest)
    {
        return new GetServiceRequestListDto(
            serviceRequest.Id,
            serviceRequest.Customer.UserName,
            serviceRequest.Description,
            serviceRequest.ScheduledDateTime,
            serviceRequest.ServiceCategories.Select(x => x.Name).ToList(),
            new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            serviceRequest.Status
            );
    }
}
