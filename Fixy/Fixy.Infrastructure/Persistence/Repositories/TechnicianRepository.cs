using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class TechnicianRepository : GenericRepository<Technician>, ITechnicianRepository
{
    private readonly DbSet<Technician> _technicians;

    public TechnicianRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _technicians = dbContext.Set<Technician>();
    }

    public async Task<bool> NationalIdExistsAsync(string nationalId)
    {
        var technician = await _technicians.FirstOrDefaultAsync(x => x.NationalId == nationalId);
        return technician != null;
    }
}
