using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;

public class CancelBookingByTechnicianCommandValidator : AbstractValidator<CancelBookingByTechnicianCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public CancelBookingByTechnicianCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
