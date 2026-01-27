using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.PriceOffers.Queries;

public static class PriceOfferDomainToPriceOfferDtoMapping
{
    public static PriceOfferDto ToPriceOfferDto(this PriceOffer priceOffer)
    {
        return new PriceOfferDto
            (
                priceOffer.Id,
                priceOffer.Technician.UserName,
                priceOffer.MinPrice,
                priceOffer.MaxPrice,
                priceOffer.CreatedAt
            );
    }
}
