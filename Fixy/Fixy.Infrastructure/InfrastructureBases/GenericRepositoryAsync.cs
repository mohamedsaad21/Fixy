using Fixy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fixy.Infrastructure.InfrastructureBases;

public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
{
    #region Fields
    private readonly ApplicationDbContext _dbContext;
    #endregion

    #region Constructors
    public GenericRepositoryAsync(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    #endregion

    #region Handle Functions
    public async Task AddAsync(T entity)
    {
        await _dbContext.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(ICollection<T> entities)
    {
        await _dbContext.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _dbContext.Database.CommitTransactionAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(ICollection<T> entities)
    {
        _dbContext.RemoveRange(entities);
        await _dbContext.SaveChangesAsync();
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

    public async Task RollBackAsync()
    {
        await _dbContext.Database.RollbackTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(ICollection<T> entities)
    {
        _dbContext.UpdateRange(entities);
        await _dbContext.SaveChangesAsync();
    }
    #endregion
}