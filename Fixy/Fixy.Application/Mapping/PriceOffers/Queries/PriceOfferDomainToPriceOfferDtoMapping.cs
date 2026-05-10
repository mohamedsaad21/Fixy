using Fixy.Application.Common.DTOs.PriceOffer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.PriceOffers.Queries;

public static class PriceOfferDomainToPriceOfferDtoMapping
{
    public static PriceOfferDto ToPriceOfferDto(this PriceOffer priceOffer, ServiceRequest serviceRequest)
    {
        return new PriceOfferDto
        {
            Id = priceOffer.Id,
            TechnicianId = priceOffer.Technician.Id,
            TechnicianUserName = priceOffer.Technician.UserName,
            AverageRating = priceOffer.Technician.AverageRating,
            Price = priceOffer.Price,
            //DistanceKm = HaversineDistance.CalculateDistance(priceOffer.Technician.TechnicianLocation.Latitude, priceOffer.Technician.TechnicianLocation.Longitude, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
            CreatedAt = priceOffer.CreatedAt
        };
    }
}