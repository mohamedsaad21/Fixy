using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SignIn;


public sealed record SignInCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
