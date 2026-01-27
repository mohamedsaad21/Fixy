using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.PriceOffers.Commands.AcceptPriceOffer;

public class AcceptPriceOfferCommandValidator : AbstractValidator<AcceptPriceOfferCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public AcceptPriceOfferCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.PriceOfferId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
