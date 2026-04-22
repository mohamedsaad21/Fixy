using Fixy.Application.Resources;
using Fixy.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.AddCategory;

public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryCommandValidator(IStringLocalizer<SharedResources> stringLocalizer, IUnitOfWork unitOfWork)
    {
        _stringLocalizer = stringLocalizer;
        _unitOfWork = unitOfWork;
        ApplyValidationRules();
        ApplyCustomValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.NameEn).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.NameAr).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }

    public void ApplyCustomValidationRules()
    {
        RuleFor(x => x.NameEn).MustAsync(async (model, key, CancellationToken) => !await _unitOfWork.ServiceCategories.IsExistsAsync(key))
            .WithMessage(_stringLocalizer[SharedResourcesKeys.CategoryAlreadyExists]);

        RuleFor(x => x.NameAr).MustAsync(async (model, key, CancellationToken) => !await _unitOfWork.ServiceCategories.IsExistsAsync(key))
            .WithMessage(_stringLocalizer[SharedResourcesKeys.CategoryAlreadyExists]);
    }
}