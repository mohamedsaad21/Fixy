using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SendResetPassword;

public sealed record SendResetPasswordCommand(string Email) : IRequest<Result>;