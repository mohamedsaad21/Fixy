namespace Fixy.Application.Features.Chatbot.Queries.GetChatbotHistory;

public class GetChatbotHistoryResponse
{
    public string UserPrompt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Response { get; set; }
    public int Code { get; set; }
    public DateTimeOffset UserPromptTime { get; set; }
    public DateTimeOffset ResponseTime { get; set; }
    public double ResponseDuration { get; set; }
    public bool EscalateToSupport { get; set; }
    public string source { get; set; }
}
