using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Notifications.Commands.SaveFcmToken;

public sealed class SaveFcmTokenCommandHandler(ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, ILogger<SaveFcmTokenCommandHandler> logger) : IRequestHandler<SaveFcmTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SaveFcmTokenCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("FCM token save failed — no current user resolved.");
            return Errors.Unauthorized;
        }

        logger.LogInformation("Saving FCM token for user. UserId: {UserId}", currentUser.Id);

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            logger.LogWarning("FCM token save failed — token is null or empty. UserId: {UserId}", currentUser.Id);
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Token))
            return false;

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            logger.LogWarning("FCM token save failed — token is null or empty. UserId: {UserId}", currentUser.Id);
            return false;
        }

        currentUser.FcmToken = request.Token;
        await userManager.UpdateAsync(currentUser);
        logger.LogInformation("FCM token saved successfully. UserId: {UserId}", currentUser.Id);
        return true;
    }
}
