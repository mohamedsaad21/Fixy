using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Notifications.Commands.SaveFcmToken;

public class SaveFcmTokenCommandValidator : AbstractValidator<SaveFcmTokenCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public SaveFcmTokenCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
