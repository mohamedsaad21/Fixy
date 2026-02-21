using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandValidator : AbstractValidator<MarkAsReadCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public MarkAsReadCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.NotificationId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
