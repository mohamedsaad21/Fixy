namespace Fixy.Infrastructure.Configurations;

public class StripeSettings
{
    public string Secretkey { get; set; }
    public string Publishablekey { get; set; }
    public string WebhookSecret { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string ConnectReturnUrl { get; set; }
    public string ConnectRefreshUrl { get; set; }
}
