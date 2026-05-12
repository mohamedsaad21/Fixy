using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Chat.Commands.MarkMessagesAsRead;

public sealed record MarkMessagesAsReadCommand(Guid BookingId) : IRequest<Result>;