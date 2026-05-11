using Fixy.Application.Features.Technicians.Queries.GetTechnicianProfileForCustomers;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void TechnicianDomainToGetTechnicianProfileForCustomersResponseMapping()
    {
        CreateMap<Technician, GetTechnicianProfileForCustomersResponse>()
            .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.ServiceCategory.Localize(src.ServiceCategory.NameAr, src.ServiceCategory.NameEn)))
            .ForMember(dest => dest.TotalCompletedJobs, opt => opt.MapFrom(src => src.CompletedBookings));
    }
}
