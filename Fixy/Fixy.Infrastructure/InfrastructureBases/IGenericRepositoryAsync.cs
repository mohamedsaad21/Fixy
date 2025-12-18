using Microsoft.EntityFrameworkCore.Storage;

namespace Fixy.Infrastructure.InfrastructureBases;

public interface IGenericRepositoryAsync<T> where T : class
{
    IQueryable<T> GetTableAsTracking();
    IQueryable<T> GetTableNoTracking();
    Task<T> GetByIdAsync(Guid? Id);
    Task AddAsync(T entity);
    Task AddRangeAsync(ICollection<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(ICollection<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(ICollection<T> entities);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollBackAsync();
    Task SaveChangesAsync();
}
