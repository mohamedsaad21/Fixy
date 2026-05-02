using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public class CancelServiceRequestCommandValidator : AbstractValidator<CancelServiceRequestCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public CancelServiceRequestCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
