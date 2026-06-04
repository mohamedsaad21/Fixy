using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public sealed record GetUnreadNotificationsCountQuery() : IRequest<Result<int>>;