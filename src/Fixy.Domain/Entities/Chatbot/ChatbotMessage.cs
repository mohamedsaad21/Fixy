namespace Fixy.Domain.Entities.Chatbot;

public class ChatbotMessage : BaseEntity
{
    public string UserPrompt { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Response { get; set; }
    public int? Code { get; set; }
    public DateTimeOffset UserPromptTime { get; set; }
    public DateTimeOffset ResponseTime { get; set; }
    public double? ResponseDuration { get; set; }
    public bool? EscalateToSupport { get; set; }
    public string? source { get; set; }
    public Guid ChatbotConversationId { get; set; }
    public ChatbotConversation ChatbotConversation { get; set; }
}
