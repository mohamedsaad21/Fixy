using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public ChangePasswordCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .NotEqual(x => x.CurrentPassword).WithErrorCode(_stringLocalizer[SharedResourcesKeys.NewPasswordSameAsCurrentPassword]);
        RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).
            Equal(x => x.NewPassword).WithMessage(_stringLocalizer[SharedResourcesKeys.PasswordNotMatchConfirmPassword]);
    }
}
