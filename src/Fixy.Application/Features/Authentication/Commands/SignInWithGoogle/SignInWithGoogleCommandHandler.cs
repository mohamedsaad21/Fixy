using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Authentication.Commands.SignInWithGoogle;

public sealed class SignInWithGoogleCommandHandler(IAuthenticationService authenticationService, ILogger<SignInWithGoogleCommandHandler> logger) : IRequestHandler<SignInWithGoogleCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(SignInWithGoogleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Google OAuth sign-in attempt initiated.");
        var authResponse = await authenticationService.AuthenticateWithGoogleAsync(request.IdToken);
        logger.LogInformation("Google OAuth sign-in completed successfully. UserId: {UserId}, Role: {Role}", authResponse.UserId, authResponse.Role);
        return authResponse;
    }
}
