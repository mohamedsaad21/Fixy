using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Email confirmation attempted. Email: {Email}", request.Email);
        
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            Log.Warning("Email confirmation failed — user not found. Email: {Email}", request.Email);
            return Errors.UserNotFound;
        }

        var isCodeValid = await authenticationService.VerifyOtpAsync(user.Id, request.Code);
        if (!isCodeValid)
        {
            Log.Warning("Email confirmation failed — invalid or expired OTP. UserId: {UserId}", user.Id);
            return Errors.InvalidCode;
        }

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        Log.Information("Email confirmed successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}