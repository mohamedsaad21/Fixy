using AutoMapper;
using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile : Profile
{
    public void ServiceRequestDomainToGetCustomerServiceRequestByIdResponseMapping()
    {
        CreateMap<ServiceRequest, GetCustomerServiceRequestByIdResponse>()
            .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.Customer.UserName))
            .ForMember(dest => dest.ServiceCategories, opt => opt.MapFrom(src => src.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto(src.Address.Country, src.Address.City, src.Address.Area, src.Address.Street, src.Address.BuildingNumber, src.Address.Latitude, src.Address.Longitude)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ServiceRequestImages))
            .ForMember(dest => dest.PriceOffers, opt => opt.MapFrom(src => src.PriceOffers))
            .ForMember(dest => dest.ScheduledDateTime, opt => opt.MapFrom(src => src.ScheduledDateTime.ToEgyptTime()));

    }
}