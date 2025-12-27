using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;

namespace Fixy.Infrastructure.Persistence.Abstracts;

public interface ITechnicianRepository : IGenericRepositoryAsync<Technician>
{
    Task<bool> NationalIdExistsAsync(string nationalId);
}
