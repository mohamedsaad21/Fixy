using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Payments;
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
    public IGenericRepository<Payment> Payments { get; private set; }
    public IGenericRepository<Dispute> Disputes { get; private set; }
    public IGenericRepository<CustomerFeedback> CustomerFeedbacks { get; private set; }
    public IGenericRepository<TechnicianFeedback> TechnicianFeedbacks { get; private set; }
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
        Payments = new GenericRepository<Payment>(dbContext);
        Disputes = new GenericRepository<Dispute>(dbContext);
        CustomerFeedbacks = new GenericRepository<CustomerFeedback>(dbContext);
        TechnicianFeedbacks = new GenericRepository<TechnicianFeedback>(dbContext);
        Notifications = new NotificationRepository(dbContext);
    }
    public void Dispose() => _dbContext.Dispose();
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}
