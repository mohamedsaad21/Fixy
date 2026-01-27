using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Commands.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RefreshTokenValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        //RuleFor(x => x.Token).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
        //    .NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}