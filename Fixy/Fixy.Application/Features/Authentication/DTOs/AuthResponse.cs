namespace Fixy.Application.Features.Authentication.DTOs;

public class AuthResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    //public DateTime ExpiresOn { get; set; }
}
