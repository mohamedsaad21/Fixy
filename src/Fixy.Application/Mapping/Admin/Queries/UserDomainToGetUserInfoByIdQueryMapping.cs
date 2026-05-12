using Fixy.Application.Features.Admin.Queries.GetUserInfoById;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.Admin;

public partial class AdminProfile
{
    public void UserDomainToGetUserInfoByIdQueryMapping()
    {
        CreateMap<ApplicationUser, GetUserInfoByIdResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.CreatedAt.ToEgyptTime()));
    }
}
