using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Commands.MarkAsRead;

public sealed record MarkAsReadCommand(Guid NotificationId) : IRequest<Result<bool>>;
