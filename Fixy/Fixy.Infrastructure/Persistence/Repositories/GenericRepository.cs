using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(T entity)
    {
        await _dbContext.AddAsync(entity);
    }

    public async Task AddRangeAsync(ICollection<T> entities)
    {
        await _dbContext.AddRangeAsync(entities);
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Remove(entity);
    }

    public async Task DeleteRangeAsync(ICollection<T> entities)
    {
        _dbContext.RemoveRange(entities);
    }

    public async Task<T> GetByIdAsync(Guid? Id)
    {
        return await _dbContext.Set<T>().FindAsync(Id);
    }

    public IQueryable<T> GetTableAsTracking()
    {
        return _dbContext.Set<T>().AsTracking().AsQueryable();
    }

    public IQueryable<T> GetTableNoTracking()
    {
        return _dbContext.Set<T>().AsNoTracking().AsQueryable();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Update(entity);
    }

    public async Task UpdateRangeAsync(ICollection<T> entities)
    {
        _dbContext.UpdateRange(entities);
    }
}