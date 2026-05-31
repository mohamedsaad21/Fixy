using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RevokeTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Token revocation initiated.");

        var token = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
        {
            Log.Warning("Token revocation failed — no user matched the provided refresh token.");
            return Errors.InvalidToken;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            Log.Warning("Token revocation failed — token is already inactive. UserId: {UserId}, TokenExpiry: {TokenExpiry}", user.Id, refreshToken.ExpiresOn);
            return Errors.InactiveToken;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        Log.Information("Refresh token revoked successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}
