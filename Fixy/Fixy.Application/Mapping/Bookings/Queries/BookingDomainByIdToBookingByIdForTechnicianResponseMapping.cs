using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings;

public partial class BookingProfile
{
    public void BookingDomainByIdToBookingByIdForTechnicianResponseMapping()
    {
        CreateMap<ServiceBooking, GetBookingByIdForTechnicianResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.ServiceRequest.CustomerId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.ServiceRequest.Customer.FirstName + " " + src.ServiceRequest.Customer.LastName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ServiceRequest.Description))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto(src.ServiceRequest.Address.Country, src.ServiceRequest.Address.City, src.ServiceRequest.Address.Area, src.ServiceRequest.Address.Street, src.ServiceRequest.Address.BuildingNumber, src.ServiceRequest.Address.Latitude, src.ServiceRequest.Address.Longitude)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ServiceRequest.ServiceRequestImages));
    }
}
