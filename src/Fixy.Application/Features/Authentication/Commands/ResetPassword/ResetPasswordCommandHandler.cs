using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordCommandHandler> logger) 
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset requested. Email: {Email}", request.Email);
        
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Password reset failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        // Check if new password is the same as the current one
        if (user.PasswordHash is not null)
        {
            var isSamePassword = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                != PasswordVerificationResult.Failed;

            if (isSamePassword)
            {
                logger.LogWarning("Password reset rejected — new password matches current password. UserId: {UserId}", user.Id);
                return Errors.PasswordPreviouslyUsed;
            }
        }

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, request.Password);

        logger.LogInformation("Password reset completed successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}
