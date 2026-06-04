using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<RevokeTokenCommandHandler> logger)
    : IRequestHandler<RevokeTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Token revocation initiated.");

        var token = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
        {
            logger.LogWarning("Token revocation failed — no user matched the provided refresh token.");
            return Errors.InvalidToken;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            logger.LogWarning("Token revocation failed — token is already inactive. UserId: {UserId}, TokenExpiry: {TokenExpiry}", user.Id, refreshToken.ExpiresOn);
            return Errors.InactiveToken;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        logger.LogInformation("Refresh token revoked successfully. UserId: {UserId}", user.Id);

        return Result.Success();
    }
}
