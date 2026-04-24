using Fixy.Application.Features.Technicians.Queries.GetTechnicianById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void TechnicianDomainToGetTechnicianByIdResponseMapping()
    {
        CreateMap<Technician, GetTechnicianByIdResponse>()
            .ForMember(dest => dest.ServiceCategoryName,
                opt => opt.MapFrom(src => src.ServiceCategory.Localize(src.ServiceCategory.NameAr, src.ServiceCategory.NameEn)))
            .ForMember(dest => dest.ServiceArea, opt => opt.MapFrom(src => src.TechnicianLocation.ServiceArea));
    }
}
