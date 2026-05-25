using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.SignInWithGoogle;

public sealed class SignInWithGoogleCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<SignInWithGoogleCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(SignInWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var authResponse = await authenticationService.AuthenticateWithGoogleAsync(request.IdToken);
        return authResponse;
    }
}
