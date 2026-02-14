using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid ServiceBookingId { get; set; }
    public ServiceBooking ServiceBooking { get; set; }
    public string StripePaymentIntentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public string Status { get; set; }
    public string Currency { get; set; }
    public decimal PlatformCommission { get; set; }
    public decimal TechnicianAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
}
