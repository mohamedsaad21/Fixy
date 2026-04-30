using Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void GetTechnicianServiceRequestByIdResponse()
    {
        CreateMap<ServiceRequest, GetTechnicianServiceRequestByIdResponse>()
            .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.Customer.UserName))
            .ForMember(dest => dest.ServiceCategories, opt => opt.MapFrom(src => src.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList()));
    }
}
