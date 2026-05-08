using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Technician : ApplicationUser
{
    public Technician()
    {
        ServiceBookings = new HashSet<ServiceBooking>();
        PriceOffers = new HashSet<PriceOffer>();
        CustomerFeedbacks = new HashSet<CustomerFeedback>();
        TechnicianFeedbacks = new HashSet<TechnicianFeedback>();
    }
    public int YearsOfExperience { get; set; }
    public TechnicianStatus Status { get; set; } = TechnicianStatus.PendingApproval;
    public string NationalId { get; set; }
    public string NationalIdCardImageUrl {  get; set; }
    public int? ComplaintsCount { get; set; }
    public int? ResponseTime { get; set; }
    public double AverageRating { get; set; } = 0;
    public string? StripeAccountId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }
    public ServiceCategory ServiceCategory { get; set; }
    public TechnicianLocation TechnicianLocation { get; set; }
    public ICollection<ServiceBooking> ServiceBookings { get; set; }
    public ICollection<PriceOffer> PriceOffers { get; set; }
    public ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
    public ICollection<TechnicianFeedback> TechnicianFeedbacks { get; set; }
    public ICollection<Payout> Payouts { get; set; }
    public ICollection<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; set; }
    public ICollection<BlockedServiceRequest> BlockedServiceRequests { get; set; }
}
