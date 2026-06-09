using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Chatbot;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public sealed class SendMessageCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IChatbotService chatbotService) : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        var conversation = await unitOfWork.ChatbotConversations.GetTableAsTracking().SingleOrDefaultAsync(x => x.UserId == user.Id);
        if (conversation == null)
        {
            conversation = new ChatbotConversation(user.Id);
            await unitOfWork.ChatbotConversations.AddAsync(conversation);
        }

        var chatbotMessages = new ChatbotMessage
        {
            UserPrompt = request.Message,
            UserPromptTime = DateTimeOffset.UtcNow,
            ChatbotConversation = conversation,
        };

        await unitOfWork.ChatbotMessages.AddAsync(chatbotMessages);

        var aiResponse = await chatbotService.SendPromptAsync(chatbotMessages);

        return new SendMessageResponse
        {
            Response = aiResponse,
        };
    }
}
