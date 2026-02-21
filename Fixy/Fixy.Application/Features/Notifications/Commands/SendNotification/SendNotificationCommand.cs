using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Commands.SendNotification;

public sealed record SendNotificationCommand
    (
        Guid UserId,
        string Type,
        Dictionary<string, object> Data
    ) : IRequest<Result<bool>>;