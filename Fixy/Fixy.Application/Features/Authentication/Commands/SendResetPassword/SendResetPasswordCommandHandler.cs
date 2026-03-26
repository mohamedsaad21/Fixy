using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.SendResetPassword;

public sealed class SendResetPasswordCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
    : IRequestHandler<SendResetPasswordCommand, Result>
{
    public async Task<Result> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (!user.EmailConfirmed)
            return Errors.EmailNotConfirmed;

        await authenticationService.SendCodeAsync(user, "reset your password", "Reset Password");
        return Result.Success();
    }
}
