using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        var unreadNotificationsCount = await unitOfWork.Notifications.GetTableNoTracking()
            .CountAsync(x => x.UserId == currentUser.Id && !x.IsRead);

        return unreadNotificationsCount;
    }
}
