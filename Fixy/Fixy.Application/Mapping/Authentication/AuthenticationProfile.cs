using AutoMapper;

namespace Fixy.Application.Mapping.Authentication;

public partial class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        RegisterCustomerCommandToCustomerMapping();
    }
}
