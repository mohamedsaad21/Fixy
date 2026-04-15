using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Payments;

public class Payout : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; }
    public string? Method { get; set; }  // VodafoneCash, OrangeCash, etc.
    public string? RecipientInfo { get; set; }  // Wallet number or bank account
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? PaymobPayoutId { get; set; }
}
