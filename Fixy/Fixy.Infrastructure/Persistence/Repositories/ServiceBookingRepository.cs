using Fixy.Domain.Entities;
using Fixy.Infrastructure.InfrastructureBases;
using Fixy.Infrastructure.Persistence.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ServiceBookingRepository : GenericRepositoryAsync<ServiceBooking>, IServiceBookingRepository
{
    private readonly DbSet<ServiceBooking> _bookings;
    public ServiceBookingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _bookings = dbContext.Set<ServiceBooking>();
    }
}
