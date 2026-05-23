namespace Fixy.Application.Contracts.ExternalServices;

public interface IChatbotService
{
    Task<string> SendPromptAsync(string prompt);
}
