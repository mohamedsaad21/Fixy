using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.EnableTwoFactor;

public sealed class EnableTwoFactorCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) : IRequestHandler<EnableTwoFactorCommand, Result<string>>
{
    public async Task<Result<string>> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        Log.Information("2FA enable requested. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            Log.Warning("2FA enable failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!user.EmailConfirmed)
        {
            Log.Warning("2FA enable failed — email not confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailNotConfirmed;
        }

        if (user.IsTwoFactorEmailEnabled)
        {
            Log.Warning("2FA enable skipped — already enabled. UserId: {UserId}", user.Id);
            return Errors.TwoFactorAlreadyEnabled;
        }

        await authenticationService.SendOtpAsync(user, "Enable 2FA", "Verifying your identity");
        Log.Information("2FA OTP dispatched successfully. UserId: {UserId}", user.Id);
        return "OTP sent to your email. Please verify to enable 2FA";
    }
}