using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById.Responses;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.PriceOffers;

public partial class PriceOfferProfile
{
    public void PriceOfferDomainToPriceOfferDtoMapping()
    {
        CreateMap<PriceOffer, PriceOfferDto>()
            .ForMember(dest => dest.TechnicianUserName, opt => opt.MapFrom(src => src.Technician.UserName))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Technician.AverageRating))
            .ForMember(dest => dest.DistanceKm, opt => 
            opt.MapFrom(src => GeoDistance.CalculateKm(src.Technician.TechnicianLocation.Latitude, 
            src.Technician.TechnicianLocation.Longitude, src.ServiceRequest.Address.Latitude, src.ServiceRequest.Address.Longitude)));
    }
}
