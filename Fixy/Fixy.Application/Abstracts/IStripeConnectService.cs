using Stripe;

namespace Fixy.Application.Abstracts;

public interface IStripeConnectService
{
    Task<string> CreateTechnicianAccountAsync(Guid technicianId, string email, string country = "US");
    Task<string> CreateOnboardingLinkAsync(string stripeAccountId, string returnUrl, string refreshUrl);
    Task<Account> GetAccountAsync(string stripeAccountId);
    Task<bool> IsAccountFullyOnboardedAsync(string stripeAccountId);
}
