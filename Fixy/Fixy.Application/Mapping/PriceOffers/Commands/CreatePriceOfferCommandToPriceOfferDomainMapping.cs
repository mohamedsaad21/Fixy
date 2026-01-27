using Fixy.Application.Features.ServiceRequests.Commands.CreatePriceOffer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.PriceOffers.Commands;

public static class CreatePriceOfferCommandToPriceOfferDomainMapping
{
    public static PriceOffer ToPriceOfferDomain(this CreatePriceOfferCommand command)
    {
        return new PriceOffer 
        { 
            ServiceRequestId = command.ServiceRequestId,
            MinPrice = command.MinPrice, 
            MaxPrice = command.MaxPrice
        };
    }
}
