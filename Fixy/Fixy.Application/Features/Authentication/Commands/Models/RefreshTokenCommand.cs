using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record RefreshTokenCommand() : IRequest<Result<AuthResponse>>;