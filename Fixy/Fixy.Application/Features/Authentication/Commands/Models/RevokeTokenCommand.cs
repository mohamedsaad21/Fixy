using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class RevokeTokenCommand : IRequest<Result>
{
    public string Token { get; set; }
}
