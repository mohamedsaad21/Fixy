using Dapper;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using System.Data;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ServiceCategoryReadRepository : IServiceCategoryReadRepository
{
    private readonly IDbConnection _db;

    public ServiceCategoryReadRepository(IDbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<List<ServiceCategory>> GetServiceCategories()
    {
        var sql = "SELECT * FROM [dbo].[ServiceCategories]";
        return _db.Query<ServiceCategory>(sql).ToList();
    }

    public async Task<ServiceCategory> GetServiceCategoryById(Guid Id)
    {
        var sql = "SELECT * FROM [dbo].[ServiceCategories] WHERE Id = @id";
        var parameters = new { id = Id };
        return _db.QuerySingle<ServiceCategory>(sql, parameters);
    }
}
