using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Users.Commands.EditUserProfile;

public class EditUserProfileCommandValidator : AbstractValidator<EditUserProfileCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public EditUserProfileCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Bio).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.ProfilePicture)
            .Must(file => file.Length <= 10 * 1024 * 1024)
            .WithMessage("File size must not exceed 10MB")
            .Must(file =>
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(extension);
            })
            .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed")
            .When(x => x.ProfilePicture != null);
    }
}
