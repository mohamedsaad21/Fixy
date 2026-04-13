using AutoMapper;

namespace Fixy.Application.Mapping.Users;

public partial class UserProfile : Profile
{
    public UserProfile()
    {
        UserDomainToGetUserByIdResponseMapping();
    }
}
