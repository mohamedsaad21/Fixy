using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Admin.Commands.UnblockCustomer;

public class UnblockCustomerCommandValidator : AbstractValidator<UnblockCustomerCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public UnblockCustomerCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
