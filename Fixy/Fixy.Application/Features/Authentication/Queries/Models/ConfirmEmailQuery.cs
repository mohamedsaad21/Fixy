using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Queries.Models;

public class ConfirmEmailQuery : IRequest<Result>
{
    public string Email { get; set; }
    public string Code { get; set; }
}
