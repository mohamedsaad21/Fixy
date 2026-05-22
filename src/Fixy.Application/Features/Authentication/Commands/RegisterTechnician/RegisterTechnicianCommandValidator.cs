using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Authentication.Commands.RegisterTechnician;

public class RegisterTechnicianCommandValidator : AbstractValidator<RegisterTechnicianCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RegisterTechnicianCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.LastName).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.Email).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).EmailAddress();

        RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0);

        RuleFor(x => x.NationalId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).Matches(@"^\d{14}$");

        RuleFor(x => x.NationalIdCardImage).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required])
            .Must(file => file.Length <= 100 * 1024 * 1024)
            .WithMessage("File size must not exceed 100MB")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            })
            .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed");

        RuleFor(x => x.ProfilePicture)
            .Must(file => file.Length <= 100 * 1024 * 1024)
            .WithMessage("File size must not exceed 100MB")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            })
            .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed")
            .When(x => x.ProfilePicture != null);

        RuleFor(x => x.Password).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty])
            .Equal(x => x.Password).WithMessage(_stringLocalizer[SharedResourcesKeys.PasswordNotMatchConfirmPassword]);
    }
}