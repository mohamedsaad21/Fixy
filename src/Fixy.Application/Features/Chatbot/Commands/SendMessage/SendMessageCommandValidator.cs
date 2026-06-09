using Fixy.Application.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    private readonly IStringLocalizer<SharedResources> _stringLocalizer;
    public SendMessageCommandValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        ApplyValidationRules();
    }

    public void ApplyValidationRules()
    {
        RuleFor(x => x.Message).NotEmpty().WithMessage(_stringLocalizer[SharedResourcesKeys.NotEmpty]);
    }
}
