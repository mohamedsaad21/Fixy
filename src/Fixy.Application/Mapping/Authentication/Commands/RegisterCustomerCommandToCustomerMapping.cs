using Fixy.Application.Features.Authentication.Commands.RegisterCustomer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Authentication;

public partial class AuthenticationProfile
{
    public void RegisterCustomerCommandToCustomerMapping()
    {
        CreateMap<RegisterCustomerCommand, Customer>();
    }
}
