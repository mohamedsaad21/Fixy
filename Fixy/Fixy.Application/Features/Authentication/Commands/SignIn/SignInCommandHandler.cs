using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Fixy.Application.Features.Authentication.Commands.SignIn;

public sealed class SignInCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
    : IRequestHandler<SignInCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Errors.EmailOrPasswordInCorrect;

        if (!user.EmailConfirmed)
            return Errors.EmailNotConfirmed;

        // Get Jwt Token
        var authResponse = new AuthResponse();

        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            authResponse.RefreshToken = activeRefreshToken.Token;
            authResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = await authenticationService.GenerateRefreshToken();
            authResponse.RefreshToken = refreshToken.Token;
            authResponse.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
        }

        var jwtSecurityToken = await authenticationService.CreateJwtToken(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var roles = await userManager.GetRolesAsync(user);
        authResponse.UserName = user.UserName;
        authResponse.Email = user.Email;
        authResponse.Role = roles.FirstOrDefault();
        authResponse.Token = accessToken;

        await authenticationService.SetTokenAndRefreshTokenInCookie(accessToken, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);

        return authResponse;
    }
}
