using Fixy.Application.Features.Users.Queries.GetUserProfileById;
using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Mapping.Users;

public partial class UserProfile
{
    public void UserDomainToGetUserByIdResponseMapping()
    {
        CreateMap<ApplicationUser, GetUserProfileByIdResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
    }
}
