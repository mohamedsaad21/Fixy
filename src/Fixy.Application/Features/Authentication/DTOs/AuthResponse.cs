using System.Text.Json.Serialization;

namespace Fixy.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public bool IsAuthenticated { get; set; }
    public string Message { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string Status { get; set; }
    public string Role { get; set; }
    [JsonIgnore]
    public string Token { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set; }
    [JsonIgnore]
    public DateTimeOffset RefreshTokenExpiration { get; set; }
    [JsonIgnore]
    public DateTime ExpiresOn { get; set; }
}
