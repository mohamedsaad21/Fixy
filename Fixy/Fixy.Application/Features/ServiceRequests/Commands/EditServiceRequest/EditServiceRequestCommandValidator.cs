using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;

public class EditServiceRequestCommandValidator : AbstractValidator<EditServiceRequestCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public EditServiceRequestCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Description).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.ScheduledDateTime).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);

        RuleFor(x => x.Address.Country).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.City).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.Area).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.Street).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
        RuleFor(x => x.Address.BuildingNumber).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]).MaximumLength(100);
    }
}
