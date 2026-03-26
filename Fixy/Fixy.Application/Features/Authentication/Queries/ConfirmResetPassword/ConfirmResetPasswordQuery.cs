using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Authentication.Queries.ConfirmResetPassword;

public sealed record ConfirmResetPasswordQuery
    (
        string Email,
        string Code
    ) : IRequest<Result>;