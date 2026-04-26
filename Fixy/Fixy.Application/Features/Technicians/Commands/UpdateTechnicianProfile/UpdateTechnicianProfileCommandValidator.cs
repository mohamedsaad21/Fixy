using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Technicians.Commands.UpdateTechnicianProfile;

public class UpdateTechnicianProfileCommandValidator : AbstractValidator<UpdateTechnicianProfileCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public UpdateTechnicianProfileCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.TechnicianId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.NationalId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).Matches(@"^\d{14}$");
        RuleFor(x => x.NationalIdCardImage).NotNull().WithMessage(_stringLocalizer[SharedResourcesKeys.Required]);
    }
}
