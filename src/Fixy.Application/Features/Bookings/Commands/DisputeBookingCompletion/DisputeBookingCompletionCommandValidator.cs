using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.DisputeBookingCompletion;

public class DisputeBookingCompletionCommandValidator : AbstractValidator<DisputeBookingCompletionCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public DisputeBookingCompletionCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Reason).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
