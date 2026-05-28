using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Password change requested. Email: {Email}", request.Email);

        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            Log.Warning("Password change failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (!await userManager.CheckPasswordAsync(user, request.CurrentPassword))
        {
            Log.Warning("Password change failed — incorrect current password. UserId: {UserId}", user.Id);
            return Errors.PasswordInCorrect;
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            Log.Warning("Password change failed — Identity error. UserId: {UserId}, Errors: {Errors}", user.Id, string.Join(", ", result.Errors.Select(e => e.Code)));
            return Errors.IdentityChangePasswordFailed;
        }

        Log.Information("Password changed successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
