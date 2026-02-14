using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public IServiceCategoryRepository ServiceCategories { get; private set; }
    public IGenericRepository<ServiceRequest> ServiceRequests { get; private set; }
    public IGenericRepository<ServiceRequestImage> ServiceRequestImages { get; private set; }
    public IGenericRepository<PriceOffer> PriceOffers { get; private set; }
    public IGenericRepository<ServiceBooking> Bookings { get; private set; }
    public ITechnicianRepository Technicians { get; private set; }
    public IGenericRepository<TechnicianLocation> TechnicianLocations { get; private set; }
    public IGenericRepository<TechnicianStripeAccount> TechnicianStripeAccounts { get; private set; }
    public IGenericRepository<TechnicianTransfer> TechnicianTransfers { get; private set; }
    public IGenericRepository<StripeWebhookEvent> StripeWebhookEvents { get; private set; }
    public IGenericRepository<PaymentRefund> PaymentRefunds { get; private set; }
    public IGenericRepository<Payment> Payments { get; private set; }
    public IGenericRepository<Dispute> Disputes { get; private set; }
    public INotificationRepository Notifications { get; private set; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        ServiceCategories = new ServiceCategoryRepository(dbContext);
        ServiceRequests = new GenericRepository<ServiceRequest>(dbContext);
        ServiceRequestImages = new GenericRepository<ServiceRequestImage>(dbContext);
        PriceOffers = new GenericRepository<PriceOffer>(dbContext);
        Bookings = new GenericRepository<ServiceBooking>(dbContext);
        Technicians = new TechnicianRepository(dbContext);
        TechnicianLocations = new GenericRepository<TechnicianLocation>(dbContext);
        TechnicianStripeAccounts = new GenericRepository<TechnicianStripeAccount>(dbContext);
        TechnicianTransfers = new GenericRepository<TechnicianTransfer>(dbContext);
        StripeWebhookEvents = new GenericRepository<StripeWebhookEvent>(dbContext);
        Payments = new GenericRepository<Payment>(dbContext);
        PaymentRefunds = new GenericRepository<PaymentRefund>(dbContext);
        Disputes = new GenericRepository<Dispute>(dbContext);
        Notifications = new NotificationRepository(dbContext);
    }
    public void Dispose() => _dbContext.Dispose();
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}
