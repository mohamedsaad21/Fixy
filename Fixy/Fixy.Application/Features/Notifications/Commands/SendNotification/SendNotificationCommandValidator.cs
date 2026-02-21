using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Notifications.Commands.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public SendNotificationCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Type).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Data).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
