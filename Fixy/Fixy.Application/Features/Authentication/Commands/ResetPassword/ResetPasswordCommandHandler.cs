using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager) 
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, request.Password);

        return Result.Success();
    }
}
