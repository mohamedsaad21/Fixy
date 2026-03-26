using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Commands.ChangePassword;

public sealed record ChangePasswordCommand
    (
        string Email, 
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
    ) : IRequest<Result>;
