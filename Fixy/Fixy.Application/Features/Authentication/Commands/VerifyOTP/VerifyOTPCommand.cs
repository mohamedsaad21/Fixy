using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.DTOs;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.VerifyOTP;

public sealed record VerifyOTPCommand(
        string Email, 
        string Code,
        bool IsEnabling2FA
    ) : IRequest<Result<AuthResponse>>;