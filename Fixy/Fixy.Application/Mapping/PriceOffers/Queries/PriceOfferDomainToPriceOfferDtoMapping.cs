using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Stripe.Terminal;

namespace Fixy.Application.Mapping.PriceOffers.Queries;

public static class PriceOfferDomainToPriceOfferDtoMapping
{
    public static PriceOfferDto ToPriceOfferDto(this PriceOffer priceOffer, ServiceRequest serviceRequest)
    {
        return new PriceOfferDto
            (
                priceOffer.Id,
                priceOffer.Technician.UserName,
                priceOffer.Technician.AverageRating,
                priceOffer.Price,
                GeoDistance.CalculateKm(priceOffer.Technician.TechnicianLocation.Latitude, priceOffer.Technician.TechnicianLocation.Longitude, serviceRequest.Address.Latitude, serviceRequest.Address.Longitude),
                priceOffer.CreatedAt
            );
    }
}
