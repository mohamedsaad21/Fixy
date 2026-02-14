using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery() : IRequest<Result<List<GetNotificationsDto>>>;
