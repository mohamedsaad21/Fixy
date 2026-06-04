using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.SendConfirmEmail;

public sealed class SendConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService, ILogger<SendConfirmEmailCommandHandler> logger)
    : IRequestHandler<SendConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Email confirmation OTP requested. Email: {Email}", request.Email);

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            logger.LogWarning("Email confirmation OTP failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        if (user.EmailConfirmed)
        {
            logger.LogWarning("Email confirmation OTP skipped — email already confirmed. UserId: {UserId}", user.Id);
            return Errors.EmailAlreadyConfirmed;
        }

        await authenticationService.SendOtpAsync(user, "confirm your account", "Confirm Account");
        logger.LogInformation("Email confirmation OTP dispatched successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
}
