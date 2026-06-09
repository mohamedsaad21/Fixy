namespace Fixy.Application.Features.Chat.Queries.GetChatMessages;

public class GetChatMessagesResponse
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public string Attachment { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public bool IsSeen { get; set; }
}