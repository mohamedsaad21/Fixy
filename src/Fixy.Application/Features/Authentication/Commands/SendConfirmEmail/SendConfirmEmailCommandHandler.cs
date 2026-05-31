using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.SendConfirmEmail;

public sealed class SendConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
    : IRequestHandler<SendConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Email confirmation OTP requested. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            Log.Warning("Email confirmation OTP failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (user.EmailConfirmed)
        {
            Log.Warning("Email confirmation OTP skipped — email already confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailAlreadyConfirmed;
        }

        await authenticationService.SendOtpAsync(user, "confirm your account", "Confirm Account");
        Log.Information("Email confirmation OTP dispatched successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
