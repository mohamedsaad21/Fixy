using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Code) : IRequest<Result>;