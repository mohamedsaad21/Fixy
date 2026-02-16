namespace Fixy.Infrastructure.Configurations;

public class PaymobSettings
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; }
    public int IntegrationIdCard { get; set; }
    public string HmacSecret { get; set; }
    public string CallbackUrl { get; set; }
    public string IframeId { get; set; }
}