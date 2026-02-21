using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Notifications.Commands.MarkAsRead;

public sealed class MarkAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<MarkAsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetCurrentUserId();

        var notification = await unitOfWork.Notifications.GetTableAsTracking().FirstOrDefaultAsync
            (x => x.Id == request.NotificationId && x.UserId == userId, cancellationToken);

        if (notification == null)
            return Errors.NotificationNotFound;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        return true;
    }
}
