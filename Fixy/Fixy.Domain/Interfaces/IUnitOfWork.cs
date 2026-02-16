using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Payments;

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
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<Dispute> Disputes { get; }
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync();
}
