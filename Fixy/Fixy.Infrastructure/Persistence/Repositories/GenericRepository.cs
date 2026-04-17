using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly FixyDbContext _dbContext;
    public GenericRepository(FixyDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(T entity) => await _dbContext.AddAsync(entity);
    public async Task AddRangeAsync(ICollection<T> entities) => await _dbContext.AddRangeAsync(entities);
    public async Task DeleteAsync(T entity) => _dbContext.Remove(entity);
    public async Task DeleteRangeAsync(ICollection<T> entities) => _dbContext.RemoveRange(entities);
    public async Task<T> Find(Expression<Func<T, bool>> criteria) => await _dbContext.Set<T>().AsTracking().SingleOrDefaultAsync(criteria);
    public async Task<T> GetByIdAsync(Guid? Id) => await _dbContext.Set<T>().FindAsync(Id);
    public IQueryable<T> GetTableAsTracking() => _dbContext.Set<T>().AsTracking().AsQueryable();
    public IQueryable<T> GetTableNoTracking() => _dbContext.Set<T>().AsNoTracking().AsQueryable();
    public async Task UpdateAsync(T entity) => _dbContext.Update(entity);
    public async Task UpdateRangeAsync(ICollection<T> entities) => _dbContext.UpdateRange(entities);
}