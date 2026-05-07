using Fixy.Application.Common.DTOs;
using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.ServiceRequests.Queries;

public static class ServiceRequestDomainToGetMyRequestPaginatedListResponseMapping
{
    public static GetMyRequestPaginatedListResponse ToGetMyRequestPaginatedListResponse(this ServiceRequest serviceRequest, IStringLocalizer<SharedResources> localizer)
    {
        return new GetMyRequestPaginatedListResponse
        {
            Id = serviceRequest.Id,
            CustomerUserName = serviceRequest.Customer.UserName,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime.ToEgyptTime(),
            ServiceCategories = serviceRequest.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList(),
            Address = new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            Status = EnumLocalizer.Localize(serviceRequest.Status, localizer)
        };
    }
}
