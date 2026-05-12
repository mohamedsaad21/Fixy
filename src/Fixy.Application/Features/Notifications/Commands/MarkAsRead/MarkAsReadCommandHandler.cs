using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fixy.Application.Features.Notifications.Commands.MarkAsRead;

public sealed class MarkAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<MarkAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = await currentUserService.GetCurrentUserId();

        var notification = await unitOfWork.Notifications.GetTableAsTracking().FirstOrDefaultAsync
            (x => x.Id == request.NotificationId && x.UserId == userId, cancellationToken);

        if (notification == null)
            return Errors.NotificationNotFound;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
