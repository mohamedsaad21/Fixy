using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record ResetPasswordCommand(string Email, string Password, string ConfirmPassword) : IRequest<Result>;