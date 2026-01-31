using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Technician : ApplicationUser
{
    public Technician()
    {
        ServiceBookings = new HashSet<ServiceBooking>();
        PriceOffers = new HashSet<PriceOffer>();
    }
    public string NationalId { get; set; }
    public int YearsOfExperience { get; set; }
    public TechnicianStatus Status { get; set; } = TechnicianStatus.PendingVerification;
    public string? ProfilePictureUrl {  get; set; }
    public string? ProfilePicturePublicId {  get; set; }
    public string NationalIdCardImageUrl {  get; set; }
    public string NationalIdCardImagePublicId {  get; set; }
    public int? TotalCompletedJobs { get; set; }
    public int? ComplaintsCount { get; set; }
    public int? ResponseTime { get; set; }
    public double? CancellationRate { get; set; }
    public double? AverageRating { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; }
    public TechnicianLocation TechnicianLocation { get; set; }
    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
    public virtual ICollection<PriceOffer> PriceOffers { get; set; }
}
