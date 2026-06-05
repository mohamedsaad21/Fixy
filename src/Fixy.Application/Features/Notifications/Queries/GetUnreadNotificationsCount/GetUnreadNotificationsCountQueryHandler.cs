using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<GetUnreadNotificationsCountQueryHandler> logger) : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Unread notifications count failed — no current user resolved.");
            return Errors.Unauthorized;
        }

        var unreadNotificationsCount = await unitOfWork.Notifications.GetTableNoTracking()
            .CountAsync(x => x.UserId == currentUser.Id && !x.IsRead);

        logger.LogInformation("Unread notifications count fetched. UserId: {UserId}, UnreadCount: {UnreadCount}", currentUser.Id, unreadNotificationsCount);

        return unreadNotificationsCount;
    }
}
