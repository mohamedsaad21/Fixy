using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.Models;

public record ConfirmEmailCommand(string Email, string Code) : IRequest<Result>;