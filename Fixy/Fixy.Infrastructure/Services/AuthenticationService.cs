using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Fixy.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly JWTSettings _jWTSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailService emailService, JWTSettings jWTSettings, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailService = emailService;
        _jWTSettings = jWTSettings;
        _httpContextAccessor = httpContextAccessor;
    }

    
    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
            roleClaims.Add(new Claim("role", role));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString()),
        }.Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer:_jWTSettings.Issuer,
            audience:_jWTSettings.Audience,
            claims:claims,
            expires:DateTime.UtcNow.AddMinutes(_jWTSettings.DurationInMinutes),
            signingCredentials:signingCredentials
            );

        return jwtSecurityToken;
    }

    public async Task<RefreshToken> GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = new RNGCryptoServiceProvider();
        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(30)
        };
    }

    public async Task SendCodeAsync(ApplicationUser user, string actionText, string reason)
    {
        // Generate  code
        var random = new Random();
        var code = random.Next(1, 1000000).ToString("D6");
        user.Code = code;
        await _userManager.UpdateAsync(user);
        // send code to user
        var message = $"This code to {actionText}: {code}";
        await _emailService.SendEmailAsync(user.Email, message, reason);
    }

    public async Task SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTime expires)
    {
        var response = _httpContextAccessor.HttpContext?.Response;

        if (response == null)
            throw new InvalidOperationException("HTTP context is not available");

        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };
        response.Cookies.Append("token", token);
        response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);
    }
}
