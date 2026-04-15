using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Payments;

public class Wallet : DatedEntity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
}
