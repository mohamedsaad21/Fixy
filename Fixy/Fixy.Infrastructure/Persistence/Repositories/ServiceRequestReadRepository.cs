using Dapper;
using Fixy.Domain.Entities;
using Fixy.Infrastructure.Persistence.Abstracts;
using System.Data;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ServiceRequestReadRepository : IServiceRequestReadRepository
{
    private readonly IDbConnection _db;

    public ServiceRequestReadRepository(IDbConnectionFactory factory)
    {
        _db = factory.CreateConnection();
    }

    public async Task<IEnumerable<ServiceRequest>> GetServiceRequestsAsync()
    {
        throw new NotImplementedException();
    }
}
