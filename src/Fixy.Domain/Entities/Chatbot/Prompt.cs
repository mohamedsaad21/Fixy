using Fixy.Domain.Entities.Identity;

namespace Fixy.Domain.Entities.Chatbot;

public class Prompt : BaseEntity
{
    public string? Description { get; set; }
    public string UserPrompt { get; set; }
    public string? Response { get; set; }
    public string? Code { get; set; }
    public DateTimeOffset UserPromptTime { get; set; }
    public DateTimeOffset? ResponseTime { get; set; }
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
