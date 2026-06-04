using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Queries.ConfirmResetPassword;

public sealed class ConfirmResetPasswordQueryHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<ConfirmResetPasswordQueryHandler> logger) : IRequestHandler<ConfirmResetPasswordQuery, Result>
{
    public async Task<Result> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset OTP confirmation attempted. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Password reset OTP confirmation failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
        {
            logger.LogWarning("Password reset OTP confirmation failed — invalid or expired code. UserId: {UserId}", user.Id);
            return Errors.InvalidCode;
        }
        logger.LogInformation("Password reset OTP confirmed successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
