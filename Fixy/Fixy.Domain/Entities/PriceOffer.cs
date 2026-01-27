using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class PriceOffer : BaseEntity
{
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public PriceOfferStatus Status { get; set; } = PriceOfferStatus.Submitted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
