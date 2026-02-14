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
    IGenericRepository<TechnicianStripeAccount> TechnicianStripeAccounts { get; }
    IGenericRepository<TechnicianTransfer> TechnicianTransfers { get; }
    IGenericRepository<StripeWebhookEvent> StripeWebhookEvents { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<PaymentRefund> PaymentRefunds { get; }
    IGenericRepository<Dispute> Disputes { get; }
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync();
}
