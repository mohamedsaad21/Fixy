using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SendConfirmEmail;

public sealed record SendConfirmEmailCommand(string Email) : IRequest<Result>;