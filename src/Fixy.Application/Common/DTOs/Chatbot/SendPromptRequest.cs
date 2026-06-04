using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.Chatbot;

public class SendPromptRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("userID")]
    public string UserId { get; set; }
    [JsonPropertyName("username")]
    public string UserName { get; set; }
    [JsonPropertyName("language")]
    public string Language { get; set; }
}
