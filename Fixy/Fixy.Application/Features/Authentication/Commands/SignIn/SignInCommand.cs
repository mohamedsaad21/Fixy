using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password) : IRequest<Result<string>>;