using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Payments;

public class Payout : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid BookingId { get; set; }
    public ServiceBooking Booking { get; set; }
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; }
    public string? Method { get; set; }  // VodafoneCash, OrangeCash, etc.
    public string? RecipientInfo { get; set; }  // Wallet number or bank account
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; set; }
    public string? PaymobPayoutId { get; set; }
}
