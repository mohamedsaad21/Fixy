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
    public string NationalId { get; set; }
    public int YearsOfExperience { get; set; }
    public TechnicianStatus Status { get; set; } = TechnicianStatus.PendingApproval;
    public string NationalIdCardImageUrl {  get; set; }
    public string NationalIdCardImagePublicId {  get; set; }
    public string? Bio {  get; set; }
    public int? ComplaintsCount { get; set; }
    public int? ResponseTime { get; set; }
    public double? AverageRating { get; set; }
    public string? StripeAccountId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? BlockedAt { get; set; }
    public ServiceCategory ServiceCategory { get; set; }
    public TechnicianLocation TechnicianLocation { get; set; }
    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
    public virtual ICollection<PriceOffer> PriceOffers { get; set; }
    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; }
    public virtual ICollection<TechnicianFeedback> TechnicianFeedbacks { get; set; }
    public virtual ICollection<Payout> Payouts { get; set; }
    public virtual ICollection<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; set; }
}
