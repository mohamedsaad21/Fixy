using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.Chatbot;

public class SendPromptRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
}
