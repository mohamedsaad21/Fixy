using Fixy.Application.Common.DTOs.Chatbot;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Chatbot;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Fixy.Infrastructure.ExternalServices;

public class ChatbotService : IChatbotService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FlaskApiSettings _flaskApiSettings;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    public ChatbotService(IHttpClientFactory httpClientFactory, FlaskApiSettings flaskApiSettings, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _httpClientFactory = httpClientFactory;
        _flaskApiSettings = flaskApiSettings;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> SendPromptAsync(ChatbotMessage chatbotMessage)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        var currentUserRole = await _currentUserService.GetCurrentUserRoleAsync();
        if(currentUser == null)
        {
            //
        }
        var requestBody = new SendPromptRequest
        {
            Query = chatbotMessage.UserPrompt,
            Email = currentUser.Email,
            Role = currentUserRole,
            UserId = currentUser.Id.ToString(),
            UserName = currentUser.UserName,
            Language = CultureInfo.CurrentCulture.Name
        };

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

        chatbotMessage.Name = result.Name;
        chatbotMessage.Description = result.Description;
        chatbotMessage.Response = result.Response;
        chatbotMessage.Code = result.Code;
        chatbotMessage.ResponseTime = DateTimeOffset.UtcNow;
        chatbotMessage.ResponseDuration = result.ResponseTime;
        chatbotMessage.EscalateToSupport = result.EscalateToSupport;
        chatbotMessage.source = result.Source;

        await _unitOfWork.SaveChangesAsync();

        return result.Response;
    }
}
