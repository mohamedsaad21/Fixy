using Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void TechnicianDomainToGetTechnicianProfileForCustomersResponseMapping()
    {
        CreateMap<Technician, GetTechnicianProfileForCustomersResponse>();
    }
}
