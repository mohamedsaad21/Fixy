using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Commands.Validators;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ConfirmEmailValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Email).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.Code).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
