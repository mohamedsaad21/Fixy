using Fixy.Domain.Enums;

namespace Fixy.Domain.Entities.Payments;

public class WalletTransaction : DatedEntity
{
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public WalletTransactionType Type { get; set; }
    public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Pending;
    public string? ReferenceId { get; set; }
    public string? StripeSessionId { get; set; }
    public string Description { get; set; }
}
