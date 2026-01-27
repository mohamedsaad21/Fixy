using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;
using Fixy.Infrastructure.Persistence.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class TechnicianLocationRepository : GenericRepositoryAsync<TechnicianLocation>, ITechnicianLocationRepository
{
    private readonly DbSet<TechnicianLocation> _locations;
    public TechnicianLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _locations = dbContext.Set<TechnicianLocation>();
    }
}
