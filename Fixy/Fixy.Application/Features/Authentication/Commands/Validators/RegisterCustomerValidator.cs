using Fixy.Application.Features.Authentication.Commands.Models;
using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Commands.Validators;

public class RegisterCustomerValidator : AbstractValidator<RegisterCustomerCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RegisterCustomerValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.NationalId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).Matches(@"^\d{14}$");

        RuleFor(x => x.NationalIdCardImage).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);

        RuleFor(x => x.Password).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .Equal(x => x.Password).WithMessage(_stringLocalizer[SharedResourcesKeys.PasswordNotMatchConfirmPassword]);
    }
}