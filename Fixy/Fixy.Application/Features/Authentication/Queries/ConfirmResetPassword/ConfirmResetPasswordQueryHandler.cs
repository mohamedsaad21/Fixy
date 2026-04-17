using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Queries.ConfirmResetPassword;

public sealed class ConfirmResetPasswordQueryHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) : IRequestHandler<ConfirmResetPasswordQuery, Result>
{
    public async Task<Result> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
            return Errors.InvalidCode;

        return Result.Success();
    }
}
