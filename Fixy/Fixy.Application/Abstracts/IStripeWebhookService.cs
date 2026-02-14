namespace Fixy.Application.Abstracts;

public interface IStripeWebhookService
{
    Task<bool> HandleWebhookAsync(string json, string signature);
}
