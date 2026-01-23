using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;
using Fixy.Infrastructure.Persistence.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ServiceRequestRepository : GenericRepositoryAsync<ServiceRequest>, IServiceRequestRepository
{
    private readonly DbSet<ServiceRequest> _serviceRequests;
    public ServiceRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _serviceRequests = dbContext.Set<ServiceRequest>();
    }
}
