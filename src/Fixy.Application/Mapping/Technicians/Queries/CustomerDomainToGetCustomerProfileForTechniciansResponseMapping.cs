using Fixy.Application.Features.Technicians.Queries.GetCustomerProfileForTechnicians;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void CustomerDomainToGetCustomerProfileForTechniciansResponseMapping()
    {
        CreateMap<Customer, GetCustomerProfileForTechniciansResponse>();
    }
}
