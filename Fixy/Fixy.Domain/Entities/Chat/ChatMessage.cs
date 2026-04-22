using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Chat;

public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; }
    public string Content { get; set; }
    public bool IsSeen { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime SeenAt { get; set; }
}