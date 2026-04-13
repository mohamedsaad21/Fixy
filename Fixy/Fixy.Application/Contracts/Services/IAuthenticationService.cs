<<<<<<< HEAD
﻿using Fixy.Domain.Entities.Identity;
=======
﻿using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
>>>>>>> feature/MFA
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Contracts.Services;

public interface IAuthenticationService
{
    Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
<<<<<<< HEAD
    Task<RefreshToken> GenerateRefreshToken();
    Task SendCodeAsync(ApplicationUser user, string actionText, string reason);
=======
    Task<AuthResponse> GetJwtToken(ApplicationUser user);
    Task<RefreshToken> GenerateRefreshToken();
    Task SendOtpAsync(ApplicationUser user, string actionText, string reason);
    Task<bool> VerifyOtpAsync(Guid userId, string code);
>>>>>>> feature/MFA
    Task SetTokenAndRefreshTokenInCookie(string token, string refreshToken, DateTime expires);
}
