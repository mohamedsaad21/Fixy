using Fixy.Domain.Entities.Chatbot;

namespace Fixy.Application.Contracts.ExternalServices;

public interface IChatbotService
{
    Task<string> SendPromptAsync(ChatbotMessage prompt);
}
