using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.EnableTwoFactor;

public sealed record EnableTwoFactorCommand(string Email) : IRequest<Result<string>>;