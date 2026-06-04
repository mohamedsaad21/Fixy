using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<ConfirmEmailCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Email confirmation attempted. Email: {Email}", request.Email);
        
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Email confirmation failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
        {
            logger.LogWarning("Email confirmation failed — invalid or expired OTP. UserId: {UserId}", user.Id);
            return Errors.InvalidCode;
        }

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        logger.LogInformation("Email confirmed successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}