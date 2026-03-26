using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Authentication.Queries.ConfirmResetPassword;

public sealed class ConfirmResetPasswordQueryHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ConfirmResetPasswordQuery, Result>
{
    public async Task<Result> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Errors.UserNotFound;

        if (user.Code != request.Code)
            return Errors.InvalidCode;

        return Result.Success();
    }
}
