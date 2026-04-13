using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;

public class DeleteServiceRequestImageByIdCommandValidator : AbstractValidator<DeleteServiceRequestImageByIdCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public DeleteServiceRequestImageByIdCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.ImageId).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
