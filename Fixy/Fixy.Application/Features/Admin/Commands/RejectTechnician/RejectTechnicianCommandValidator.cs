using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public class RejectTechnicianCommandValidator : AbstractValidator<RejectTechnicianCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public RejectTechnicianCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.TechnicianId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Reason).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
