using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.EnableTwoFactor;

public sealed class EnableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) : IRequestHandler<EnableTwoFactorCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Errors.UserNotFound;

        if (!user.EmailConfirmed)
            return Errors.EmailNotConfirmed;

        if(user.IsTwoFactorEmailEnabled)
            return Errors.TwoFactorAlreadyEnabled;

        await authenticationService.SendOtpAsync(user, "Enable 2FA", "Verifying your identity");
        return "OTP sent to your email. Please verify to enable 2FA";
    }
}
