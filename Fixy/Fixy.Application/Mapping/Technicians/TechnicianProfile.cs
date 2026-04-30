using AutoMapper;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile : Profile
{
    public TechnicianProfile()
    {
        TechnicianDomainToGetTechnicianByIdResponseMapping();
        TechnicianDomainToGetTechnicianProfileForCustomersResponseMapping();
        CustomerDomainToGetCustomerProfileForTechniciansResponseMapping();
        GetTechnicianServiceRequestByIdResponse();
    }
}
