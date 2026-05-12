using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.RefreshToken;

public sealed record RefreshTokenCommand() : IRequest<Result<AuthResponse>>;