using Fixy.Domain.Entities;

namespace Fixy.Domain.Interfaces;

public interface ITechnicianRepository : IGenericRepository<Technician>
{
    Task<bool> NationalIdExistsAsync(string nationalId);
}
