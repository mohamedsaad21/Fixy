using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public class SendPromptCommandValidator : AbstractValidator<SendPromptCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public SendPromptCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Prompt).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
