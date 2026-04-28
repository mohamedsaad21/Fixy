using Fixy.Application.Common.DTOs;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.ServiceRequests;

public static class RequestDomainListToRequestListMapping
{
    public static GetServiceRequestListResponse ToServiceRequestListResponse(this ServiceRequest serviceRequest, IStringLocalizer<SharedResources> localizer)
    {
        return new GetServiceRequestListResponse
        {
            Id = serviceRequest.Id,
            CustomerUserName = serviceRequest.Customer.UserName,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime,
            ServiceCategories = serviceRequest.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList(),
            Address = new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            Status = EnumLocalizer.Localize(serviceRequest.Status, localizer)
        };
    }
}
