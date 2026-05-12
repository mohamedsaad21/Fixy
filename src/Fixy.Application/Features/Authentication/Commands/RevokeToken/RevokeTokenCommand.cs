using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.RevokeToken;

public sealed record RevokeTokenCommand() : IRequest<Result>;