using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) 
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Password reset requested. Email: {Email}", request.Email);
        
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            Log.Warning("Password reset failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        // Check if new password is the same as the current one
        if (user.PasswordHash is not null)
        {
            var isSamePassword = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                != PasswordVerificationResult.Failed;

            if (isSamePassword)
            {
                Log.Warning("Password reset rejected — new password matches current password. UserId: {UserId}", user.Id);
                return Errors.PasswordPreviouslyUsed;
            }
        }

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, request.Password);

        Log.Information("Password reset completed successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}
