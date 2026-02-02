using Fixy.Domain.Entities;

namespace Fixy.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IServiceCategoryRepository ServiceCategories { get; }
    IGenericRepository<ServiceRequest> ServiceRequests { get; }
    IGenericRepository<ServiceRequestImage> ServiceRequestImages { get; }
    IGenericRepository<PriceOffer> PriceOffers { get; }
    IGenericRepository<ServiceBooking> Bookings { get; }
    ITechnicianRepository Technicians { get; }
    IGenericRepository<TechnicianLocation> TechnicianLocations { get; }
    Task<int> SaveChangesAsync();
}
