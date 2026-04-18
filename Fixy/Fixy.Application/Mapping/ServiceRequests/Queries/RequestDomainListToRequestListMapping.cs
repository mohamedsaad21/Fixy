using Fixy.Application.Common.DTOs;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;

namespace Fixy.Application.Mapping.ServiceRequests;

public static class RequestDomainListToRequestListMapping
{
    public static GetServiceRequestListResponse ToServiceRequestListResponse(this ServiceRequest serviceRequest)
    {
        return new GetServiceRequestListResponse
        {
            Id = serviceRequest.Id,
            CustomerUserName = serviceRequest.Customer.UserName,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime,
            ServiceCategories = serviceRequest.ServiceCategories.Select(x => x.Name).ToList(),
            Address = new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            Status = (ServiceRequestStatus)serviceRequest.Status
        };
    }
}
