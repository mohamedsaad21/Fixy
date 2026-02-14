using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class TechnicianTransfer : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public string StripeTransferId { get; set; }
    public decimal Amount { get; set; }
    public string Currency {  get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
