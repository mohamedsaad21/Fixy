using Fixy.Application.Features.Users.Commands.EditUserProfile;
using Fixy.Domain.Entities.Identity;

namespace Fixy.Application.Mapping.Users;

public partial class UserProfile
{
    public void EditUserProfileCommandToUserDomainMapping()
    {
        CreateMap<EditUserProfileCommand, ApplicationUser>();
    }
}
