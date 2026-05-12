using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.DisableTwoFactor;

public sealed record DisableTwoFactorCommand(string Email) : IRequest<Result>;