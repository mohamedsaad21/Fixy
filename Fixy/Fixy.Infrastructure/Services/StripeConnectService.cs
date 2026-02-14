using Fixy.Application.Abstracts;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Fixy.Infrastructure.Services;

public class StripeConnectService : IStripeConnectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AccountService _accountService;
    private readonly AccountLinkService _accountLinkService;
    private readonly StripeSettings _stripeSettings;

    public StripeConnectService(IUnitOfWork unitOfWork, StripeSettings stripeSettings)
    {
        _unitOfWork = unitOfWork;
        _accountService = new AccountService();
        _accountLinkService = new AccountLinkService();
        _stripeSettings = stripeSettings;
    }

    /// <summary>
    /// Create a Stripe Express connected account for a technician/seller
    /// </summary>
    public async Task<string> CreateTechnicianAccountAsync(Guid technicianId, string email, string country = "US")
    {
        // Check if account already exists
        var existing = await _unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == technicianId);

        if (existing.StripeAccountId != null)
        {
            return existing.StripeAccountId;
        }

        // Create Stripe Express account
        var options = new AccountCreateOptions
        {
            Type = "express", // Express is recommended for marketplaces
            Country = country,
            Email = email,
            Capabilities = new AccountCapabilitiesOptions
            {
                // Enable ability to receive transfers
                Transfers = new AccountCapabilitiesTransfersOptions
                {
                    Requested = true
                },
                // Optional: enable card payments if sellers need to charge directly
                CardPayments = new AccountCapabilitiesCardPaymentsOptions
                {
                    Requested = true
                }
            },
            BusinessType = "individual", // or "company"
        };

        var requestOptions = new RequestOptions
        {
            ApiKey = _stripeSettings.SecretKey
        };

        var account = await _accountService.CreateAsync(options, requestOptions);

        // Store in database
        var technicianAccount = new TechnicianStripeAccount
        {
            TechnicianId = technicianId,
            StripeAccountId = account.Id,
            OnboardingStatus = "pending",
            ChargesEnabled = account.ChargesEnabled,
            PayoutsEnabled = account.PayoutsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TechnicianStripeAccounts.AddAsync(technicianAccount);
        await _unitOfWork.SaveChangesAsync();

        return account.Id;
    }

    /// <summary>
    /// Create onboarding link for technician to complete Stripe verification
    /// </summary>
    public async Task<string> CreateOnboardingLinkAsync(string stripeAccountId, string returnUrl, string refreshUrl)
    {
        var options = new AccountLinkCreateOptions
        {
            Account = stripeAccountId,
            RefreshUrl = refreshUrl, // URL if onboarding needs to be refreshed
            ReturnUrl = returnUrl,   // URL after successful onboarding
            Type = "account_onboarding",
        };

        var requestOptions = new RequestOptions
        {
            ApiKey = _stripeSettings.SecretKey
        };
        var link = await _accountLinkService.CreateAsync(options, requestOptions);
        return link.Url;
    }

    /// <summary>
    /// Get Stripe account details
    /// </summary>
    public async Task<Account> GetAccountAsync(string stripeAccountId)
    {
        return await _accountService.GetAsync(stripeAccountId);
    }

    /// <summary>
    /// Check if account is fully onboarded and ready to receive payments
    /// </summary>
    public async Task<bool> IsAccountFullyOnboardedAsync(string stripeAccountId)
    {
        var account = await _accountService.GetAsync(stripeAccountId);

        return account.ChargesEnabled &&
               account.PayoutsEnabled &&
               account.DetailsSubmitted;
    }
}