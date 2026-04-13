using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.DisableTwoFactor;

public sealed class DisableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<DisableTwoFactorCommand, Result>
{
    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Errors.UserNotFound;

        if (!user.EmailConfirmed)
            return Errors.EmailNotConfirmed;

        if (!user.IsTwoFactorEmailEnabled)
            return Errors.TwoFactorAlreadyDisabled;

        user.IsTwoFactorEmailEnabled = false;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
}
