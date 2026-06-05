using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Notifications.Queries;
using Fixy.Application.Resources;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStringLocalizer<SharedResources> localizer, ILogger<GetNotificationsQueryHandler> logger) : IRequestHandler<GetNotificationsQuery, Result<PaginatedResult<GetNotificationsResponse>>>
{
    public async Task<Result<PaginatedResult<GetNotificationsResponse>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = await currentUserService.GetCurrentUserId();
        logger.LogInformation("Fetching notifications. UserId: {UserId}, Page: {PageNumber}, PageSize: {PageSize}", userId, request.PageNumber, request.PageSize);
        var notifications = unitOfWork.Notifications.GetTableNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).AsQueryable();
        var result = await notifications.Select(x => x.ToGetNotificationsResponse(localizer)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        logger.LogInformation("Notifications fetched successfully. UserId: {UserId}, TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}", userId, result.TotalCount, result.CurrentPage, result.PageSize);
        return result;
    }
}
