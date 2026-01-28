using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandValidator : AbstractValidator<RejectBookingPriceChangeCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RejectBookingPriceChangeCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
