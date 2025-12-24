using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public sealed record RegisterCustomerCommand(
    string FullName, string UserName, string Email, string Password, string ConfirmPassword
    ) : IRequest<Result>;