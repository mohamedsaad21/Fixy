using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Chatbot;

public class ChatbotConversation : BaseEntity
{
    public ChatbotConversation(Guid userId)
    {
        UserId = userId;
        ChatbotMessages = new HashSet<ChatbotMessage>();
    }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<ChatbotMessage> ChatbotMessages { get; set; }
}
