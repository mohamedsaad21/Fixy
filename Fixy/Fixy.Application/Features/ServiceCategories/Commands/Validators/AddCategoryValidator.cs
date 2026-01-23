using Fixy.Application.Features.ServiceCategories.Commands.Models;
using Fixy.Application.Resources;
using Fixy.Infrastructure.Persistence.Abstracts;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceCategories.Commands.Validators;

public class AddCategoryValidator : AbstractValidator<AddCategoryCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    private readonly IServiceCategoryRepository _serviceCategoryRepository;

    public AddCategoryValidator(IStringLocalizer<SharedResources> stringLocalizer, IServiceCategoryRepository serviceCategoryRepository)
    {
        _stringLocalizer = stringLocalizer;
        _serviceCategoryRepository = serviceCategoryRepository;
        ApplyValidationRules();
        ApplyCustomValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }

    public void ApplyCustomValidationRules()
    {
        RuleFor(x => x.Name).MustAsync(async (model, key, CancellationToken) => !await _serviceCategoryRepository.IsExistsAsync(key))
            .WithMessage(_stringLocalizer[SharedResourcesKeys.CategoryAlreadyExists]);
    }
}