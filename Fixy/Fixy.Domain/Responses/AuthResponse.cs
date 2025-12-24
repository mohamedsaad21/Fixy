namespace Fixy.Domain.Responses;

public class AuthResponse
{
    public AuthResponse(string userName, string email, List<string> roles, string token, DateTime expiresOn)
    {
        UserName = userName;
        Email = email;
        Roles = roles;
        Token = token;
        ExpiresOn = expiresOn;
    }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
}
