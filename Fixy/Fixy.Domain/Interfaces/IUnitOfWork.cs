using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.SP.TechnicianAvailableRequests;

namespace Fixy.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IServiceCategoryRepository ServiceCategories { get; }
    IGenericRepository<ServiceRequest> ServiceRequests { get; }
    IGenericRepository<ServiceRequestImage> ServiceRequestImages { get; }
    IGenericRepository<PriceOffer> PriceOffers { get; }
    IGenericRepository<ServiceBooking> Bookings { get; }
    IGenericRepository<ServiceBookingImage> ServiceBookingImages { get; }
    ITechnicianRepository Technicians { get; }
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<TechnicianLocation> TechnicianLocations { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<Dispute> Disputes { get; }
    IGenericRepository<CustomerFeedback> CustomerFeedbacks { get; }
    IGenericRepository<TechnicianFeedback> TechnicianFeedbacks { get; }
    IGenericRepository<Payout> Payouts { get; }
    IGenericRepository<OtpCode> OtpCodes { get; }
    IConversationRepository Conversations { get; }
    IGenericRepository<ChatMessage> ChatMessages { get; }
    INotificationRepository Notifications { get; }
    IServiceRequestReadRepository ServiceRequestReadRepository { get; }
    Task<int> SaveChangesAsync();
}
