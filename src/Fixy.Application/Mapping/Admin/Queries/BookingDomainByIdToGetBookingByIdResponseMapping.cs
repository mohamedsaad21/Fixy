using Fixy.Application.Features.Admin.Queries.GetBookingById;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;

namespace Fixy.Application.Mapping.Admin;

public partial class AdminProfile
{
    public void BookingDomainByIdToGetBookingByIdResponseMapping()
    {
        CreateMap<ServiceBooking, GetBookingByIdResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.ServiceRequest.CustomerId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.ServiceRequest.Customer.FirstName + " " + src.ServiceRequest.Customer.LastName))
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician.FirstName + " " + src.Technician.LastName))
            .ForMember(dest => dest.CustomerUserName, opt => opt.MapFrom(src => src.ServiceRequest.Customer.UserName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ServiceRequest.Description))
            .ForMember(dest => dest.ServiceCategory, opt => opt.MapFrom(src => src.Technician.ServiceCategory.Localize(src.Technician.ServiceCategory.NameAr, src.Technician.ServiceCategory.NameEn)))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.AgreedPrice))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToEgyptTime()));
    }
}
