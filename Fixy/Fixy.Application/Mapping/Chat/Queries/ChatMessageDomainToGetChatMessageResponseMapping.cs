using Fixy.Application.Features.Chat.Queries.GetChatMessages;
using Fixy.Domain.Entities.Chat;

namespace Fixy.Application.Mapping.Chat.Queries;

public static class ChatMessageDomainToGetChatMessageResponseMapping
{
    public static GetChatMessagesResponse ToChatMessageResponse(this ChatMessage chatMessage)
    {
        return new GetChatMessagesResponse
        {
            Id = chatMessage.Id,
            SenderId = chatMessage.SenderId,
            Content = chatMessage.Content,
            SentAt = chatMessage.SentAt
        };
    }
}
