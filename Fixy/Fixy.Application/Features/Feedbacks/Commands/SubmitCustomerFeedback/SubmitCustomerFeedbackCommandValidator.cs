using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;

public class SubmitCustomerFeedbackCommandValidator : AbstractValidator<SubmitCustomerFeedbackCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public SubmitCustomerFeedbackCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.BookingId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.JobDurationMinutes).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .GreaterThan(0).WithMessage(_stringLocalizer[SharedResourcesKeys.JobDurationMinutesMustBeGreaterThanZero]);

        RuleFor(x => x.ResponseTimeMinutes).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .GreaterThan(0).WithMessage(_stringLocalizer[SharedResourcesKeys.ResponseTimeMinutesMustBeGreaterThanZero]);

        RuleFor(x => x.PunctualityRating).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.ProfessionalismRating).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.CommunicationRating).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.CleanlinessRating).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.SatisfactionScore).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.CostSatisfaction).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .InclusiveBetween(1, 5).WithMessage(_stringLocalizer[SharedResourcesKeys.RatingMustBeBetweenOneandFive]);

        RuleFor(x => x.WeatherCondition).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
