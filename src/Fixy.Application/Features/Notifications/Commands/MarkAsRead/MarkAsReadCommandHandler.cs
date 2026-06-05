using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Notifications.Commands.MarkAsRead;

public sealed class MarkAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<MarkAsReadCommandHandler> logger)
    : IRequestHandler<MarkAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = await currentUserService.GetCurrentUserId();

        logger.LogInformation("Marking notification as read. NotificationId: {NotificationId}, UserId: {UserId}", request.NotificationId, userId);

        var notification = await unitOfWork.Notifications.GetTableAsTracking().FirstOrDefaultAsync
            (x => x.Id == request.NotificationId && x.UserId == userId, cancellationToken);

        if (notification == null)
        {
            logger.LogWarning("Mark notification as read failed — notification not found or does not belong to user. NotificationId: {NotificationId}, UserId: {UserId}", request.NotificationId, userId);
            return Errors.NotificationNotFound;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Notification marked as read successfully. NotificationId: {NotificationId}, UserId: {UserId}", request.NotificationId, userId);

        return Result.Success();
    }
}
