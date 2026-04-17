using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Payments;

public class Payout : BaseEntity
{
    public Guid TechnicianId { get; set; }
    public Technician Technician { get; set; }
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; }
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public string? StripeTransferId { get; set; }
    public string? StripePayoutId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
}
