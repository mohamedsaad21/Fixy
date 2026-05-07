using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Fixy.Application.Features.Admin.Queries.GetUserInfoById;

public sealed class GetUserInfoByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper) : IRequestHandler<GetUserInfoByIdQuery, Result<GetUserInfoByIdResponse>>
{
    public async Task<Result<GetUserInfoByIdResponse>> Handle(GetUserInfoByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
            return Errors.UserNotFound;

        var result = mapper.Map<GetUserInfoByIdResponse>(user);
        var roles = await userManager.GetRolesAsync(user);
        result.Role = roles.FirstOrDefault()!;
        if(user is Technician technician)
        {
            result.NationalId = technician.NationalId;
            result.NationalIdCardImageUrl = technician.NationalIdCardImageUrl;
            result.AverageRating = technician.AverageRating;

        } else if(user is Customer customer)
        {
            result.NationalId = customer.NationalId;
        }
        return result;
    }
}
