using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;

public class AddServiceRequestImagesCommandValidator : AbstractValidator<AddServiceRequestImagesCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public AddServiceRequestImagesCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.ServiceRequestId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
        RuleFor(x => x.Images).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
