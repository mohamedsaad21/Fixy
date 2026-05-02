using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
namespace Fixy.Domain.Entities;

public class ServiceBooking : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid PriceOfferId { get; set; }
    public PriceOffer PriceOffer { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal? ProposedPrice { get; set; }
    public DateTime? PriceChangeRequestedAt { get; set; }
    public string? PriceChangeNotes { get; set; }
    public bool HasRequestedPriceChange { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public ServiceBookingStatus Status { get; set; } = ServiceBookingStatus.InProgress;
    public string? CompletionNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? CancelledById { get; set; }
    public ApplicationUser CancelledBy { get; set; }
    public CustomerCancellationReason? CustomerCancellationReason { get; set; }
    public TechnicianCancellationReason? TechnicianCancellationReason { get; set; }
    public string? CancellationNote { get; set; }
    public bool IsCustomerConfirmed { get; set; }
    public DateTime? CustomerConfirmedAt { get; set; }
    public bool IsEvaluated { get; set; }
    public Payment Payment { get; set; }
    public CustomerFeedback CustomerFeedback { get; set; }
    public TechnicianFeedback TechnicianFeedback { get; set; }
    public Payout Payout { get; set; }
    public List<ServiceBookingImage> ServiceBookingImages { get; set; }
}
