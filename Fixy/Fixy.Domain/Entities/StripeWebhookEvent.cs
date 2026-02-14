namespace Fixy.Domain.Entities;

public class StripeWebhookEvent : BaseEntity
{
    public string StripeEventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public bool Processed { get; set; } = false;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? EventData { get; set; }
}
