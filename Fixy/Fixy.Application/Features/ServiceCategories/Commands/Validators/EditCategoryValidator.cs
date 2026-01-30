using Fixy.Application.Features.ServiceCategories.Commands.Models;
using Fixy.Application.Resources;
using Fixy.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.Validators;

public class EditCategoryValidator : AbstractValidator<EditCategoryCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IUnitOfWork _unitOfWork;

    public EditCategoryValidator(IStringLocalizer<SharedResources> stringLocalizer, IUnitOfWork unitOfWork)
    {
        _stringLocalizer = stringLocalizer;
        _unitOfWork = unitOfWork;
        ApplyValidationRules();
        ApplyCustomValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }

    public void ApplyCustomValidationRules()
    {
        RuleFor(x => x.Name).MustAsync(async (model, key, CancellationToken) => !await _unitOfWork.ServiceCategories.IsExistsExcludeSelfAsync(model.Id, key))
            .WithMessage(_stringLocalizer[SharedResourcesKeys.CategoryAlreadyExists]);
    }
}