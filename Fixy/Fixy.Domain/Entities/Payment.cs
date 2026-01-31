using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal TechnicianAmount { get; set; }
    public DateTime? PaidAt { get; set; }
}
