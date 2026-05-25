using Fixy.Domain.Entities.Identity;
﻿using Fixy.Application.Features.Authentication.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Contracts.Services;

public interface IAuthenticationService
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    Task<RefreshToken> GenerateRefreshToken();
    Task<AuthResponse> GetJwtToken(ApplicationUser user);
    Task SendOtpAsync(ApplicationUser user, string actionText, string reason);
    Task<bool> VerifyOtpAsync(Guid userId, string code);
    Task SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTimeOffset expires);
    Task<AuthResponse> AuthenticateWithGoogleAsync(string accessToken, string idToken);
}
