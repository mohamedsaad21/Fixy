using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Chatbot;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public sealed class SendPromptCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IChatbotService chatbotService) : IRequestHandler<SendPromptCommand, Result<SendPromptResponse>>
{
    public async Task<Result<SendPromptResponse>> Handle(SendPromptCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        var prompt = new Prompt
        {
            ApplicationUserId = user.Id,
            UserPrompt = request.Prompt,
            UserPromptTime = DateTimeOffset.UtcNow
        };
        await unitOfWork.Prompts.AddAsync(prompt);

        var aiResponse = await chatbotService.SendPromptAsync(request.Prompt);

        prompt.Response = aiResponse;
        prompt.ResponseTime = DateTimeOffset.UtcNow;
        await unitOfWork.SaveChangesAsync();

        return new SendPromptResponse
        {
            Response = aiResponse,
        };
    }
}
