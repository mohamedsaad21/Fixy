using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Application.Mapping.PriceOffers.Queries;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.ServiceRequests.Queries;

public static class ServiceRequestDomainToGetServiceRequestByIdResponseMapping
{
    public static GetCustomerServiceRequestByIdResponse ToServiceRequestByIdResponse(this ServiceRequest serviceRequest, IStringLocalizer<SharedResources> localizer)
    {
        return new GetCustomerServiceRequestByIdResponse
        {
            Id = serviceRequest.Id,
            CustomerUserName = serviceRequest.Customer.UserName,
            Description = serviceRequest.Description,
            ScheduledDateTime = serviceRequest.ScheduledDateTime,
            ServiceCategories = serviceRequest.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList(),
            Address = new AddressDto(serviceRequest.Address.Country, serviceRequest.Address.City, serviceRequest.Address.Area, serviceRequest.Address.Street, serviceRequest.Address.BuildingNumber, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            Status = EnumLocalizer.Localize(serviceRequest.Status, localizer),
            Images = serviceRequest.ServiceRequestImages.Select(x => new ImageDto {Id = x.Id, ImageUrl = x.ImageUrl }).ToList(),
            PriceOffers = serviceRequest.PriceOffers
            .Select(x => x.ToPriceOfferDto(serviceRequest)).OrderByDescending(x => x.AverageRating).ThenBy(x => x.DistanceKm)
            .ThenBy(x => x.Price).ToList()
        };
    }
}