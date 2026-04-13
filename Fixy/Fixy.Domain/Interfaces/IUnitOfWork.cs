using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;
<<<<<<< HEAD
=======
using Fixy.Domain.Entities.Identity;
>>>>>>> feature/MFA
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
    IGenericRepository<CustomerFeedback> CustomerFeedbacks { get; }
    IGenericRepository<TechnicianFeedback> TechnicianFeedbacks { get; }
    IGenericRepository<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; }
    IGenericRepository<Payout> Payouts { get; }
<<<<<<< HEAD
=======
    IGenericRepository<OtpCode> OtpCodes { get; }
>>>>>>> feature/MFA
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync();
}
