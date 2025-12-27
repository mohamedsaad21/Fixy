using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record SendResetPasswordCommand(string Email) : IRequest<Result>;