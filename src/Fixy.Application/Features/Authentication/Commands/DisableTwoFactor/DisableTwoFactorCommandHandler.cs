using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.DisableTwoFactor;

public sealed class DisableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager, ILogger<DisableTwoFactorCommandHandler> logger) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("2FA disable requested. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("2FA disable failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("2FA disable failed — email not confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailNotConfirmed;
        }

        if (!user.IsTwoFactorEmailEnabled)
        {
            logger.LogWarning("2FA disable skipped — already disabled. UserId: {UserId}", user.Id);
            return Errors.TwoFactorAlreadyDisabled;
        }

        user.IsTwoFactorEmailEnabled = false;
        await userManager.UpdateAsync(user);
        logger.LogInformation("2FA disabled successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
