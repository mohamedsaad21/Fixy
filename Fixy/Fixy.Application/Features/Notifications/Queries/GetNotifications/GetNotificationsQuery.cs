using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery() : IRequest<Result<List<GetNotificationsDto>>>;
