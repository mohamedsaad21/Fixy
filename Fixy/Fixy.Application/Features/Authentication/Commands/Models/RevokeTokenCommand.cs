using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record RevokeTokenCommand(string Token) : IRequest<Result>;