using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Authentication.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (!await userManager.CheckPasswordAsync(user, request.CurrentPassword))
            return Errors.PasswordInCorrect;

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return Errors.IdentityChangePasswordFailed;

        return Result.Success();
    }
}
