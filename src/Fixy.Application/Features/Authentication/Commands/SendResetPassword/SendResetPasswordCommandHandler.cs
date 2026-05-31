using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using static Serilog.Log;

namespace Fixy.Application.Features.Authentication.Commands.SendResetPassword;

public sealed class SendResetPasswordCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
    : IRequestHandler<SendResetPasswordCommand, Result>
{
    public async Task<Result> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Password reset OTP requested. Email: {Email}", request.Email);
        
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            Log.Warning("Password reset OTP failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!user.EmailConfirmed)
        {
            Log.Warning("Password reset OTP failed — email not confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailNotConfirmed;
        }

        await authenticationService.SendOtpAsync(user, "reset your password", "Reset Password");
        Log.Information("Password reset OTP dispatched successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
