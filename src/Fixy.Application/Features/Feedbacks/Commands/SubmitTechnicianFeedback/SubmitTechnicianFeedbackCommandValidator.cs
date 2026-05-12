using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public class SubmitTechnicianFeedbackCommandValidator : AbstractValidator<SubmitTechnicianFeedbackCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public SubmitTechnicianFeedbackCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.CustomerBehaviorRating).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.ClarityOfIssue).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.SafetyAndEnvironment).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.CustomerPunctuality).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);
    }
}
