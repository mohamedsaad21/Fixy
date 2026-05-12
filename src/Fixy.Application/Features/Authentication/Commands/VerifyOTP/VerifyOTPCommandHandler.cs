using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.VerifyOTP;

public sealed class VerifyOTPCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) : IRequestHandler<VerifyOTPCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Errors.UserNotFound;

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
            return Errors.InvalidCode;

        if (request.IsEnabling2FA)
        {
            user.IsTwoFactorEmailEnabled = true;
            await userManager.UpdateAsync(user);
            return new AuthResponse { Message = "Two-factor authentication enabled successfully." };
        }

        // Get Jwt Token
        var authResponse = await authenticationService.GetJwtToken(user);
        await authenticationService.SetTokenAndRefreshTokenInCookie(authResponse.Token, authResponse.RefreshToken, authResponse.RefreshTokenExpiration);
        return authResponse;
    }
}
