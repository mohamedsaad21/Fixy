using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Password change requested. Email: {Email}", request.Email);

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            logger.LogWarning("Password change failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!await userManager.CheckPasswordAsync(user, request.CurrentPassword))
        {
            logger.LogWarning("Password change failed — incorrect current password. UserId: {UserId}", user.Id);
            return Errors.PasswordInCorrect;
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            logger.LogWarning("Password change failed — Identity error. UserId: {UserId}, Errors: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Code)));
            return Errors.IdentityChangePasswordFailed;
        }

        logger.LogInformation("Password changed successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
