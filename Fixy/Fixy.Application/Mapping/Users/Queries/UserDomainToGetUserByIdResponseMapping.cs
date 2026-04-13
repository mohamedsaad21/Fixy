using Fixy.Application.Features.Users.Queries.GetUserById;
using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Mapping.Users;

public partial class UserProfile
{
    public void UserDomainToGetUserByIdResponseMapping()
    {
        CreateMap<ApplicationUser, GetUserByIdResponse>();
    }
}
