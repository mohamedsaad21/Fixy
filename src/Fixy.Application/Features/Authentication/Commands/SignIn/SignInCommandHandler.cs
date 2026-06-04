using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.SignIn;

public sealed class SignInCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<SignInCommandHandler>logger)
    : IRequestHandler<SignInCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sign-in attempt. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            logger.LogWarning("Sign-in failed — invalid email or password. Email: {Email}", request.Email);
            return Errors.EmailOrPasswordInCorrect;
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("Sign-in failed — email not confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailNotConfirmed;
        }

        if (user.IsTwoFactorEmailEnabled)
        {
            await authenticationService.SendOtpAsync(user, "Login", "Verifying your identity");
            logger.LogInformation("Sign-in paused — 2FA OTP dispatched. UserId: {UserId}", user.Id);
            return new AuthResponse
            {
                Message = "OTP sent to your email"
            };
        }
        var authResponse = await authenticationService.GetJwtToken(user);
        await authenticationService.SetTokenAndRefreshTokenInCookie(authResponse.Token, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);
        logger.LogInformation("Sign-in completed successfully. UserId: {UserId}, Role: {Role}", user.Id, authResponse.Role);
        return authResponse;
    }
}
