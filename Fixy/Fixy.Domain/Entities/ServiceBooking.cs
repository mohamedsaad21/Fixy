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
    public DateTime ScheduledDateTime { get; set; }
    public ServiceBookingStatus Status { get; set; } = ServiceBookingStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Payment Payment { get; set; }
}
