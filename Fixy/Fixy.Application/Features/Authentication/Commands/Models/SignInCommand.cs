using Fixy.Application.Bases;
using Fixy.Domain.Responses;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class SignInCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
