using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public class CreateServiceRequestCommandValidator : AbstractValidator<CreateServiceRequestCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public CreateServiceRequestCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.ScheduledDateTime).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.ServiceCategoriesIds).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.Address.Country).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.City).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.Area).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.Street).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.BuildingNumber).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
    }
}
