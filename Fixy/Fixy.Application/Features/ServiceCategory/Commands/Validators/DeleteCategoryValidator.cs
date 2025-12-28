using Fixy.Application.Features.ServiceCategory.Commands.Models;
using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategory.Commands.Validators;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public DeleteCategoryValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}