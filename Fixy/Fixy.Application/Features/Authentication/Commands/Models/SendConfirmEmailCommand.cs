using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class SendConfirmEmailCommand : IRequest<Result>
{
    public string Email { get; set; }
}
