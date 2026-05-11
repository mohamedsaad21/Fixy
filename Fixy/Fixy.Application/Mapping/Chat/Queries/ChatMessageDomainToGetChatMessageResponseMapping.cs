using Fixy.Application.Features.Chat.Queries.GetChatMessages;
using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Helpers;

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
            Attachment = chatMessage.Attachment,
            SentAt = chatMessage.SentAt.ToEgyptTime()
        };
    }
}