using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using MediatR;

namespace Fixy.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResponse>>
{
    public async Task<Result<GetCurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var role = await currentUserService.GetCurrentUserRoleAsync();

        return new GetCurrentUserResponse
        {
            Id = currentUser.Id,
            Email = currentUser.Email,
            Role = role
        };
    }
}
