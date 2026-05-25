using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SignInWithGoogle;

public sealed record SignInWithGoogleCommand
    (
        string IdToken
    ) : IRequest<Result<AuthResponse>>;