using Fixy.Application.Features.Technicians.Queries.GetTechnicianById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void TechnicianDomainToGetTechnicianByIdResponseMapping()
    {
        CreateMap<Technician, GetTechnicianByIdResponse>();
    }
}
