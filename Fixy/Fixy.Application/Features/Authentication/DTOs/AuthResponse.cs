using System.Text.Json.Serialization;

namespace Fixy.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public string Message { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
<<<<<<< HEAD
=======
    public string ProfilePictureUrl { get; set; }
>>>>>>> feature/MFA
    //public List<string> Roles { get; set; }
    public string Role { get; set; }
    [JsonIgnore]
    public string Token { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set; }
    [JsonIgnore]
    public DateTime RefreshTokenExpiration { get; set; }
    [JsonIgnore]
    public DateTime ExpiresOn { get; set; }
}
