using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Password, string ConfirmPassword) : IRequest<Result>;