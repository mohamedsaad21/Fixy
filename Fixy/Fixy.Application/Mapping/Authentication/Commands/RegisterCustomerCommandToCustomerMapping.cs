using Fixy.Application.Features.Authentication.Commands.RegisterCustomer;
using Fixy.Application.Features.Authentication.Queries.Results;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Authentication;

public partial class AuthenticationProfile
{
    public void RegisterCustomerCommandToCustomerMapping()
    {
        CreateMap<RegisterCustomerCommand, Customer>();
        CreateMap<Customer, GetCustomersResponse>();
    }
}
