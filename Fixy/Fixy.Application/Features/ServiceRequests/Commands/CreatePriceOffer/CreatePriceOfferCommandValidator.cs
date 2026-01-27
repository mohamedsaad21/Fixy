using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreatePriceOffer;

public class CreatePriceOfferCommandValidator : AbstractValidator<CreatePriceOfferCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public CreatePriceOfferCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.MinPrice).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .GreaterThan(0).WithMessage(_stringLocalizer[SharedResourcesKeys.PriceMustBeGreaterThanZero]);
        RuleFor(x => x.MaxPrice).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .GreaterThan(0).WithMessage(_stringLocalizer[SharedResourcesKeys.PriceMustBeGreaterThanZero]);
    }
}
