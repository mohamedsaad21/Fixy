using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public sealed record SendMessageCommand(string Message) : IRequest<Result<SendMessageResponse>>;