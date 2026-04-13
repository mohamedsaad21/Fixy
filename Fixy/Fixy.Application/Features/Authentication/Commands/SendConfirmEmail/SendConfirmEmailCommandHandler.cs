using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Commands.SendConfirmEmail;

public sealed class SendConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
    : IRequestHandler<SendConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (user.EmailConfirmed)
            return Errors.EmailAlreadyConfirmed;

        await authenticationService.SendCodeAsync(user, "confirm your account", "Confirm Account");
        return Result.Success();
    }
}
