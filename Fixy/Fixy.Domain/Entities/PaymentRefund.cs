namespace Fixy.Domain.Entities;

public class PaymentRefund : BaseEntity
{
    public Guid PaymentId { get; set; }
    public string StripeRefundId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "pending"; // pending, succeeded, failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Payment Payment { get; set; }
}
