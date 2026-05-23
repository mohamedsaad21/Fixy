using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Chatbot.Commands.SendPrompt;

public sealed record SendPromptCommand(string Prompt) : IRequest<Result<SendPromptResponse>>;