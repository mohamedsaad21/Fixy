using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;
using Serilog;

namespace Fixy.Application.Features.Authentication.Commands.SignInWithGoogle;

public sealed class SignInWithGoogleCommandHandler(IAuthenticationService authenticationService) : IRequestHandler<SignInWithGoogleCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(SignInWithGoogleCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Google OAuth sign-in attempt initiated.");
        var authResponse = await authenticationService.AuthenticateWithGoogleAsync(request.IdToken);
        Log.Information("Google OAuth sign-in completed successfully. UserId: {UserId}, Role: {Role}", authResponse.UserId, authResponse.Role);
        return authResponse;
    }
}
