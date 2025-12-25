using Fixy.Application.Bases;
using Fixy.Domain.Responses;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string Token { get; set; }
}
