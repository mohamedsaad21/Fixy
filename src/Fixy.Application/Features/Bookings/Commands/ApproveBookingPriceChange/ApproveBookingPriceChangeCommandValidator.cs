using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public class ApproveBookingPriceChangeCommandValidator : AbstractValidator<ApproveBookingPriceChangeCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ApproveBookingPriceChangeCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
