using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public class DeleteServiceRequestCommandValidator : AbstractValidator<DeleteServiceRequestCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;

    public DeleteServiceRequestCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
