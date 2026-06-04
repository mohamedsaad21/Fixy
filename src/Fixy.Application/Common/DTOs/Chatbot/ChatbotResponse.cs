using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.Chatbot;

public class ChatbotResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("section")]
    public string Section { get; set; }
    [JsonPropertyName("response_time")]
    public double ResponseTime { get; set; }
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("response")]
    public string Response { get; set; }
    [JsonPropertyName("escalate_to_support")]
    public bool EscalateToSupport { get; set; }
    [JsonPropertyName("source")]
    public string Source { get; set; }
}
