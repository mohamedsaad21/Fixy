using Fixy.Application.Common.DTOs.Chatbot;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Infrastructure.Configurations;
using System.Text;
using System.Text.Json;

namespace Fixy.Infrastructure.ExternalServices;

public class ChatbotService : IChatbotService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FlaskApiSettings _flaskApiSettings;
    public ChatbotService(IHttpClientFactory httpClientFactory, FlaskApiSettings flaskApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _flaskApiSettings = flaskApiSettings;
    }

    public async Task<string> SendPromptAsync(string prompt)
    {
        var requestBody = new SendPromptRequest { Prompt = prompt };

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_flaskApiSettings.ChatbotUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(_flaskApiSettings.Timeout);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ChatbotResponse>(json);

        if (result == null)
            throw new HttpRequestException("No response from Flask API.");

        return result.Response;
    }
}
