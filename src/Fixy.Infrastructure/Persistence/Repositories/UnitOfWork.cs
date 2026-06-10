using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Entities.Chatbot;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Interfaces;
using Fixy.Domain.SP.TechnicianAvailableRequests;
using Fixy.Domain.SP.TechnicianCashCommissionsOwed;
using Fixy.Infrastructure.Persistence.SP.TechnicianAvailableRequests;
using Fixy.Infrastructure.Persistence.SP.TechnicianCashCommissionsOwed;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FixyDbContext _dbContext;

    public IServiceCategoryRepository ServiceCategories { get; private set; }
    public IGenericRepository<ServiceRequest> ServiceRequests { get; private set; }
    public IGenericRepository<BlockedServiceRequest> BlockedServiceRequests { get; private set; }
    public IGenericRepository<ServiceRequestImage> ServiceRequestImages { get; private set; }
    public IGenericRepository<PriceOffer> PriceOffers { get; private set; }
    public IGenericRepository<ServiceBooking> Bookings { get; private set; }
    public IGenericRepository<ServiceBookingImage> ServiceBookingImages { get; private set; }
    public ITechnicianRepository Technicians { get; private set; }
    public IGenericRepository<Customer> Customers { get; private set; }
    public IGenericRepository<TechnicianLocation> TechnicianLocations { get; private set; }
    public IGenericRepository<Payment> Payments { get; private set; }
    public IGenericRepository<Dispute> Disputes { get; private set; }
    public IGenericRepository<CustomerFeedback> CustomerFeedbacks { get; private set; }
    public IGenericRepository<TechnicianFeedback> TechnicianFeedbacks { get; private set; }
    public IGenericRepository<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; private set; }
    public IGenericRepository<Payout> Payouts { get; private set; }
    public IGenericRepository<OtpCode> OtpCodes { get; private set; }
    public IConversationRepository Conversations { get; private set; }
    public IGenericRepository<ChatMessage> ChatMessages { get; private set; }
    public INotificationRepository Notifications { get; private set; }
    public IServiceRequestReadRepository ServiceRequestReadRepository { get; private set; }
    public ITechnicianCommissionOwedReadRepository TechnicianCommissionOwedReadRepository { get; private set; }
    public IGenericRepository<ChatbotConversation> ChatbotConversations { get; private set; }
    public IGenericRepository<ChatbotMessage> ChatbotMessages { get; private set; }
    public IGenericRepository<ApplicationUser> Users { get; private set; }

    public UnitOfWork(FixyDbContext dbContext)
    {
        _dbContext = dbContext;
        ServiceCategories = new ServiceCategoryRepository(dbContext);
        ServiceRequests = new GenericRepository<ServiceRequest>(dbContext);
        BlockedServiceRequests = new GenericRepository<BlockedServiceRequest>(dbContext);
        ServiceRequestImages = new GenericRepository<ServiceRequestImage>(dbContext);
        PriceOffers = new GenericRepository<PriceOffer>(dbContext);
        Bookings = new GenericRepository<ServiceBooking>(dbContext);
        ServiceBookingImages = new GenericRepository<ServiceBookingImage>(dbContext);
        Technicians = new TechnicianRepository(dbContext);
        Customers = new GenericRepository<Customer>(dbContext);
        TechnicianLocations = new GenericRepository<TechnicianLocation>(dbContext);
        Payments = new GenericRepository<Payment>(dbContext);
        Disputes = new GenericRepository<Dispute>(dbContext);
        CustomerFeedbacks = new GenericRepository<CustomerFeedback>(dbContext);
        TechnicianFeedbacks = new GenericRepository<TechnicianFeedback>(dbContext);
        TechnicianCommissionsOwed = new GenericRepository<TechnicianCommissionOwed>(dbContext);
        Payouts = new GenericRepository<Payout>(dbContext);
        OtpCodes = new GenericRepository<OtpCode>(dbContext);
        Conversations = new ConversationRepository(dbContext);
        ChatMessages = new GenericRepository<ChatMessage>(dbContext);
        Notifications = new NotificationRepository(dbContext);
        ServiceRequestReadRepository = new ServiceRequestReadRepository(dbContext);
        TechnicianCommissionOwedReadRepository = new TechnicianCommissionOwedReadRepository(dbContext);
        ChatbotConversations = new GenericRepository<ChatbotConversation>(dbContext);
        ChatbotMessages = new GenericRepository<ChatbotMessage>(dbContext);
        Users = new GenericRepository<ApplicationUser>(dbContext);
    }
    public void Dispose() => _dbContext.Dispose();
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}
