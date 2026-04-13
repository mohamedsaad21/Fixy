using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandValidator : AbstractValidator<MarkBookingCompletedCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public MarkBookingCompletedCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.CompletionNotes).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .MaximumLength(1000).WithMessage(_stringLocalizer[SharedResourcesKeys.CompletionNotesMaxLength1000]);
        RuleFor(x => x.CompletionImages).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .Must(x => x.Count <= 5).WithMessage(_stringLocalizer[SharedResourcesKeys.CompletionImagesMax5])
            .ForEach(image =>
            {
                image.Must(x => x.Length > 0).WithMessage(SharedResourcesKeys.EmptyImage);
            });
    }
}
