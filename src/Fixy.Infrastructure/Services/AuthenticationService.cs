using Fixy.Application.Common.Helpers;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using Fixy.Infrastructure.Configurations;
using Google.Apis.Auth;
using Google.Apis.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
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
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, 
        IEmailService emailService, JWTSettings jWTSettings, 
        IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> localizer,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _jWTSettings = jWTSettings;
        _httpContextAccessor = httpContextAccessor;
        _localizer = localizer;
        _configuration = configuration;
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
            CreatedOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(30)
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
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        await _unitOfWork.OtpCodes.AddAsync(otp);
        await _unitOfWork.SaveChangesAsync();

        var message = $"This code to {actionText}: {code}";
        await _emailService.SendEmailAsync(user.Email, message, reason);
    }

    public async Task<bool> VerifyOtpAsync(Guid userId, string code)
    {
        var otp = await _unitOfWork.OtpCodes
            .GetTableAsTracking().Where(o => o.ApplicationUserId == userId && o.Code == code && !o.IsUsed)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync();

        if (otp == null || otp.ExpiresAt < DateTimeOffset.UtcNow)
            return false;

        otp.IsUsed = true; // mark as used so it can't be reused
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTimeOffset expires)
    {
        var response = _httpContextAccessor.HttpContext?.Response;

        if (response == null)
            throw new InvalidOperationException("HTTP context is not available");

        var tokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime()
        };
        response.Cookies.Append("token", token, tokenCookieOptions);
        response.Cookies.Append("refreshToken", refreshToken, tokenCookieOptions);
    }

    public async Task<AuthResponse> GetJwtToken(ApplicationUser user)
    {
        var authResponse = new AuthResponse();

        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            authResponse.RefreshToken = activeRefreshToken.Token;
            authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = await GenerateRefreshToken();
            authResponse.RefreshToken = refreshToken.Token;
            authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
        }

        var jwtSecurityToken = await CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var roles = await _userManager.GetRolesAsync(user);
        authResponse.UserId = user.Id;
        authResponse.UserName = user.UserName;
        authResponse.Email = user.Email;
        authResponse.ProfilePictureUrl = user.ProfilePictureUrl;
        authResponse.Status = user switch
        {
            Customer customer => EnumLocalizer.Localize(customer.Status, _localizer),
            Technician technician => EnumLocalizer.Localize(technician.Status, _localizer),
            _ => _localizer[SharedResourcesKeys.AdminActiveStatus]
        };
        authResponse.Role = roles.FirstOrDefault();
        authResponse.Token = accessToken;
        authResponse.IsAuthenticated = true;
        return authResponse;
    }

    public async Task<AuthResponse> AuthenticateWithGoogleAsync(string idToken)
    {
        // Validate the ID token and retrieve the payload
        var payload = await ValidateGoogleTokenAsync(idToken);

        // Check if the user already exists in the database
        var user = await _userManager.FindByEmailAsync(payload.Email);

        // If user doesn't exist, throw UnauthorizedAccessException
        if (user == null)
            throw new UnauthorizedAccessException("User not registered.");

        // Get a token
        var authResponse = await GetJwtToken(user);
        await SetTokenAndRefreshTokenInCookie(authResponse.Token, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

        return authResponse;
    }

    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _configuration["Authentication:Google:ClientId"] }
        };
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        if (payload == null || string.IsNullOrEmpty(payload.Email))
            throw new UnauthorizedAccessException("Invalid Google Token");

        return payload;
    }
}