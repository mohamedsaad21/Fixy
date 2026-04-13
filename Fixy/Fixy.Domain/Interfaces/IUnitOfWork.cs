using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;

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
    IGenericRepository<TechnicianLocation> TechnicianLocations { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<Dispute> Disputes { get; }
    IGenericRepository<CustomerFeedback> CustomerFeedbacks { get; }
    IGenericRepository<TechnicianFeedback> TechnicianFeedbacks { get; }
    IGenericRepository<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; }
    IGenericRepository<Payout> Payouts { get; }
    IGenericRepository<OtpCode> OtpCodes { get; }
    INotificationRepository Notifications { get; }
    Task<int> SaveChangesAsync();
}
