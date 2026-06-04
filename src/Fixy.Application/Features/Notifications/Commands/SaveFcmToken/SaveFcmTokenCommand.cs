using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Notifications.Commands.SaveFcmToken;

public sealed record SaveFcmTokenCommand(string? Token) : IRequest<Result<bool>>;