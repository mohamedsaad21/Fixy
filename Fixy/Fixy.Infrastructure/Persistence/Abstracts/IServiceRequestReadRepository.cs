using Fixy.Domain.Entities;

namespace Fixy.Infrastructure.Persistence.Abstracts;

public interface IServiceRequestReadRepository
{
    Task<IEnumerable<ServiceRequest>> GetServiceRequestsAsync();
}
