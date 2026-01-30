using Fixy.Domain.Entities;

namespace Fixy.Domain.Interfaces;

public interface IServiceCategoryRepository : IGenericRepository<ServiceCategory>
{
    Task<bool> IsExistsAsync(string Name);
    Task<bool> IsExistsExcludeSelfAsync(Guid Id, string Name);
}
