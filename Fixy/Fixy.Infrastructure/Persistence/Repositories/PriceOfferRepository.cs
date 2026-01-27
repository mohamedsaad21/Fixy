using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;
using Fixy.Infrastructure.Persistence.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class PriceOfferRepository : GenericRepositoryAsync<PriceOffer>, IPriceOfferRepository
{
    private readonly DbSet<PriceOffer> _priceOffers;
    public PriceOfferRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _priceOffers = dbContext.Set<PriceOffer>();
    }
}
