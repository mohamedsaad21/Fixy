using Fixy.Application.Resources;
using Fixy.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.EditCategory;

public class EditCategoryCommandValidator : AbstractValidator<EditCategoryCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IUnitOfWork _unitOfWork;

    public EditCategoryCommandValidator(IStringLocalizer<SharedResources> stringLocalizer, IUnitOfWork unitOfWork)
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