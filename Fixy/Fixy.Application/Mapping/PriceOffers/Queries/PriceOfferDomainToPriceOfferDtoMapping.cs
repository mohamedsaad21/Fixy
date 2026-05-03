using Fixy.Application.Common.DTOs.PriceOffer;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.PriceOffers;

public partial class PriceOfferProfile
{
    public void PriceOfferDomainToPriceOfferDtoMapping()
    {
        CreateMap<PriceOffer, PriceOfferDto>()
            .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.Technician.Id))
            .ForMember(dest => dest.TechnicianFullName, opt => opt.MapFrom(src => src.Technician.FirstName + " " + src.Technician.LastName))
            .ForMember(dest => dest.TechnicianUserName, opt => opt.MapFrom(src => src.Technician.UserName))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Technician.AverageRating))
            .ForMember(dest => dest.DistanceKm, opt => 
            opt.MapFrom(src => HaversineDistance.CalculateDistance(src.Technician.TechnicianLocation.Latitude, 
            src.Technician.TechnicianLocation.Longitude, src.ServiceRequest.Address.Latitude, src.ServiceRequest.Address.Longitude)));
    }
}
