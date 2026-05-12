using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public class RequestBookingPriceChangeCommandValidator : AbstractValidator<RequestBookingPriceChangeCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RequestBookingPriceChangeCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.NewProposedPrice).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .GreaterThan(0).WithMessage(_stringLocalizer[SharedResourcesKeys.PriceMustBeGreaterThanZero]);
        RuleFor(x => x.Notes).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
