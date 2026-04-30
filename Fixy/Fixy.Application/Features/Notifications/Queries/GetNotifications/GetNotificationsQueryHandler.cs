using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Notifications.Queries;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<PaginatedResult<GetNotificationsResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }

    public async Task<Result<PaginatedResult<GetNotificationsResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var notifications = _unitOfWork.Notifications.GetTableNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).AsQueryable();
        var FilterQuery = await notifications.Select(x => x.ToGetNotificationsResponse(_localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return FilterQuery;
    }
}
