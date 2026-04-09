using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed record ProcessCallbackCommand() : IRequest<Result<bool>>;