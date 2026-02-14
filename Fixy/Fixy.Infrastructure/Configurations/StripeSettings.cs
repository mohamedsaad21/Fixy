namespace Fixy.Infrastructure.Configurations;

public class StripeSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public decimal PlatformCommissionPercentage { get; set; } = 10;
}