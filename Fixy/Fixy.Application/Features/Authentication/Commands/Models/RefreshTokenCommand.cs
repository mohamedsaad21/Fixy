using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record RefreshTokenCommand(string Token) : IRequest<Result<AuthResponse>>;