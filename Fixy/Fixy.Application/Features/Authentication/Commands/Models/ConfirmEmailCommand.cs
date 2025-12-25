using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public class ConfirmEmailCommand : IRequest<Result>
{
    public string Email { get; set; }
    public string Code { get; set; }
}
