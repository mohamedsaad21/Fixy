using Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.Technicians;

public partial class TechnicianProfile
{
    public void GetTechnicianServiceRequestByIdResponse()
    {
        CreateMap<ServiceRequest, GetTechnicianServiceRequestByIdResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
            .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.Customer.UserName))
            .ForMember(dest => dest.ServiceCategories, opt => opt.MapFrom(src => src.ServiceCategories.Select(x => x.Localize(x.NameAr, x.NameEn)).ToList()))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ServiceRequestImages))
            .ForMember(dest => dest.ScheduledDateTime, opt => opt.MapFrom(src => src.ScheduledDateTime.ToEgyptTime()));
    }
}
