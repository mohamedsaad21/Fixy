using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed record ProcessCallbackCommand(IQueryCollection QueryData) : IRequest<Result<bool>>;