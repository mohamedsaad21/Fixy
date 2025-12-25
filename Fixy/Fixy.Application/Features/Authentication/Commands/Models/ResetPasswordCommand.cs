using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
