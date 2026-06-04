using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.VerifyOTP;

public sealed class VerifyOTPCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<VerifyOTPCommandHandler> logger) : IRequestHandler<VerifyOTPCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("OTP verification attempted. Email: {Email}, Purpose: {Purpose}", request.Email, request.IsEnabling2FA ? "Enable2FA" : "SignIn");

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("OTP verification failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
        {
            logger.LogWarning("OTP verification failed — invalid or expired code. UserId: {UserId}, Purpose: {Purpose}", user.Id, request.IsEnabling2FA ? "Enable2FA" : "SignIn");
            return Errors.InvalidCode;
        }

        if (request.IsEnabling2FA)
        {
            user.IsTwoFactorEmailEnabled = true;
            await userManager.UpdateAsync(user);
            logger.LogInformation("2FA enabled successfully via OTP verification. UserId: {UserId}", user.Id);
            return new AuthResponse { Message = "Two-factor authentication enabled successfully." };
        }

        // Get Jwt Token
        var authResponse = await authenticationService.GetJwtToken(user);
        await authenticationService.SetTokenAndRefreshTokenInCookie(authResponse.Token, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);
        logger.LogInformation("OTP verification successful — JWT issued. UserId: {UserId}, Role: {Role}", user.Id, authResponse.Role);
        return authResponse;
    }
}
