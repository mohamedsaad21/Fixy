using System.Text.Json.Serialization;

namespace Fixy.Application.Common.DTOs.Chat;

public class MessageContent
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    [JsonPropertyName("attachment")]
    public string? Attachment { get; set; }
}
