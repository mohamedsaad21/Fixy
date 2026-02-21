using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.Notifications.Queries;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<GetNotificationsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<GetNotificationsDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var notifications = await _unitOfWork.Notifications.GetTableNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();
        return notifications.Select(x => x.ToNotificationsDto()).ToList();
    }
}
