using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ServiceCategoryRepository : GenericRepository<ServiceCategory>, IServiceCategoryRepository
{
    private readonly DbSet<ServiceCategory> _serviceCategories;

    public ServiceCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _serviceCategories = dbContext.Set<ServiceCategory>();
    }

    public async Task<bool> IsExistsAsync(string Name)
    {
        var serviceCategory = await _serviceCategories.FirstOrDefaultAsync(x => x.Name == Name);
        return serviceCategory != null;
    }

    public async Task<bool> IsExistsExcludeSelfAsync(Guid Id, string Name)
    {
        var serviceCategory = await _serviceCategories.FirstOrDefaultAsync(x => x.Name == Name && x.Id != Id);
        return serviceCategory != null;
    }
}
