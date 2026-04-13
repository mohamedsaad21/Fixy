using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Contracts.Services;

public interface IAuthenticationService
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    Task<AuthResponse> GetJwtToken(ApplicationUser user);
    Task<RefreshToken> GenerateRefreshToken();
    Task SendOtpAsync(ApplicationUser user, string actionText, string reason);
    Task<bool> VerifyOtpAsync(Guid userId, string code);
    Task SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTime expires);
}
