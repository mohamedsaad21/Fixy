using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Notifications.Commands.SaveFcmToken;

public sealed class SaveFcmTokenCommandHandler(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager) : IRequestHandler<SaveFcmTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SaveFcmTokenCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        currentUser.FcmToken = request.Token;
        await userManager.UpdateAsync(currentUser);
        return true;
    }
}
