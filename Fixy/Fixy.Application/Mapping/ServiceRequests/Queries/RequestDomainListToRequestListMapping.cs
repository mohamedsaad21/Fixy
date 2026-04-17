using Fixy.Application.Common.DTOs;
using Fixy.Domain.Enums;
using Fixy.Domain.SP.TechnicianAvailableRequests;

namespace Fixy.Application.Mapping.ServiceRequests;

public static class RequestDomainListToRequestListMapping
{
    public static GetServiceRequestListDto ToServiceRequestListDto(this ServiceRequestSpResult serviceRequest)
    {
        return new GetServiceRequestListDto(
            serviceRequest.Id,
            serviceRequest.UserName,
            serviceRequest.Description,
            serviceRequest.ScheduledDateTime,
            //serviceRequest.ServiceCategories.Select(x => x.Name).ToList(),
            //new AddressDto(serviceRequest.Country, serviceRequest.City, serviceRequest.Area, serviceRequest.Street, serviceRequest.BuildingNumber, serviceRequest.Latitude, serviceRequest.Longitude),
            (ServiceRequestStatus)serviceRequest.Status
            );
    }
}
