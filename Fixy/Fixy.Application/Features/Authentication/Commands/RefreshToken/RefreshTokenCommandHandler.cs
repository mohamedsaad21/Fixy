using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Features.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager,
    IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return Errors.InactiveToken;

        refreshToken.RevokedOn = DateTime.UtcNow;

        var newRefreshToken = await authenticationService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);
        var jwtSecurityToken = await authenticationService.CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        await authenticationService.SetTokenAndRefreshTokenInCookie(accessToken, newRefreshToken.Token, newRefreshToken.ExpiresOn);
        var roles = await userManager.GetRolesAsync(user);
        return new AuthResponse
        {
            UserName = user.UserName,
            Email = user.Email,
            Role = roles.FirstOrDefault(),
            Token = accessToken,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.ExpiresOn
        };
    }
}