using Fixy.Domain.Entities;

namespace Fixy.Infrastructure.Persistence.Abstracts;

public interface IServiceCategoryReadRepository
{
    Task<List<ServiceCategory>> GetServiceCategories();
    Task<ServiceCategory> GetServiceCategoryById(Guid Id);
}
