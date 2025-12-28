using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;

namespace Fixy.Infrastructure.Persistence.Abstracts;

public interface IServiceCategoryRepository : IGenericRepositoryAsync<ServiceCategory>
{
    Task<bool> IsExistsAsync(string Name);
    Task<bool> IsExistsExcludeSelfAsync(Guid Id, string Name);
}
