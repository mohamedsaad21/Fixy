using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.EnableTwoFactor;

public sealed class EnableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<EnableTwoFactorCommandHandler> logger) : IRequestHandler<EnableTwoFactorCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("2FA enable requested. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("2FA enable failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("2FA enable failed — email not confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailNotConfirmed;
        }

        if (user.IsTwoFactorEmailEnabled)
        {
            logger.LogWarning("2FA enable skipped — already enabled. UserId: {UserId}", user.Id);
            return Errors.TwoFactorAlreadyEnabled;
        }

        await authenticationService.SendOtpAsync(user, "Enable 2FA", "Verifying your identity");
        logger.LogInformation("2FA OTP dispatched successfully. UserId: {UserId}", user.Id);
        return "OTP sent to your email. Please verify to enable 2FA";
    }
}