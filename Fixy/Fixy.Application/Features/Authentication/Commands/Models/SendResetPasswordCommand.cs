using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class SendResetPasswordCommand : IRequest<Result>
{
    public string Email { get; set; }
}
