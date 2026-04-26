using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Admin.Commands.BlockTecnhnician;

public class BlockTecnhnicianCommandValidator : AbstractValidator<BlockTecnhnicianCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public BlockTecnhnicianCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
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
