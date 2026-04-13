using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Fixy.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly JWTSettings _jWTSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailService emailService, JWTSettings jWTSettings, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
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

    public async Task SendOtpAsync(ApplicationUser user, string actionText, string reason)
    {
        // Generate  code
        var code = new Random().Next(1, 1000000).ToString("D6");
        var otp = new OtpCode
        {
            ApplicationUserId = user.Id,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        await _unitOfWork.OtpCodes.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();
        // send code to user
        var message = $"This code to {actionText}: {code}";
        await _emailService.SendEmailAsync(user.Email, message, reason);
    }

    public async Task<bool> VerifyOtpAsync(Guid userId, string code)
    {
        var otp = await _unitOfWork.OtpCodes
            .GetTableAsTracking().Where(o => o.ApplicationUserId == userId && o.Code == code && !o.IsUsed)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync();

        if (otp == null || otp.ExpiresAt < DateTime.UtcNow)
            return false;

        otp.IsUsed = true; // mark as used so it can't be reused
        await _unitOfWork.SaveChangesAsync();
        return true;
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
