using AutoMapper;

namespace Fixy.Application.Mapping.PriceOffers;

public partial class PriceOfferProfile : Profile
{
    public PriceOfferProfile()
    {
        PriceOfferDomainToPriceOfferDtoMapping();
    }
}
