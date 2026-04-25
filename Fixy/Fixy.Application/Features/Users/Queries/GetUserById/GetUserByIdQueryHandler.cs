using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
            return Errors.UserNotFound;

        var response = mapper.Map<GetUserByIdResponse>(user);

        var roles = await userManager.GetRolesAsync(user);
        response.Role = roles.FirstOrDefault()!;
        return response;
    }
}
