using Fixy.Application.Bases;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Authentication.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RevokeTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            return Errors.InvalidToken;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
            return Errors.InactiveToken;

        refreshToken.RevokedOn = DateTime.UtcNow;

        await userManager.UpdateAsync(user);

        return Result.Success();
    }
}
