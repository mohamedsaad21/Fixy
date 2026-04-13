using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
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

        if (user.IsTwoFactorEmailEnabled)
        {
            await authenticationService.SendOtpAsync(user, "Login", "Verifying your identity");
            return new AuthResponse
            {
                Message = "OTP sent to your email"
            };
        }
        var authResponse = await authenticationService.GetJwtToken(user);
        await authenticationService.SetTokenAndRefreshTokenInCookie(authResponse.Token, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);
        return authResponse;
    }
}
