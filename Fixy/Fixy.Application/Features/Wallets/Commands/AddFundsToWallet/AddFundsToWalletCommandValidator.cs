using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Wallets.Commands.AddFundsToWallet;

public class AddFundsToWalletCommandValidator : AbstractValidator<AddFundsToWalletCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public AddFundsToWalletCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Amount).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(100m, 1500m).WithMessage(_stringLocalizer[SharedResourcesKeys.PriceMustBeBetween100And1500]);
    }
}
