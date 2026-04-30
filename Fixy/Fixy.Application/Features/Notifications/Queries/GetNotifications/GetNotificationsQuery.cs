using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using MediatR;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery
    (
        int PageNumber,
        int PageSize
    ) : IRequest<Result<PaginatedResult<GetNotificationsResponse>>>;
