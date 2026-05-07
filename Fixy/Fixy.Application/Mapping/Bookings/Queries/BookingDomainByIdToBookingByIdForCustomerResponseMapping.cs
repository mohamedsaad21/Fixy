using Fixy.Application.Common.DTOs.ServiceRequest;
using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings;

public partial class BookingProfile
{
    public void BookingDomainByIdToBookingByIdForCustomerResponseMapping()
    {
        CreateMap<ServiceBooking, GetBookingByIdForCustomerResponse>()
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician.FirstName + " " + src.Technician.LastName))
            .ForMember(dest => dest.TechnicianProfilePicture, opt => opt.MapFrom(src => src.Technician.ProfilePictureUrl))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ServiceRequest.Description))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto(src.ServiceRequest.Address.Country, src.ServiceRequest.Address.City, src.ServiceRequest.Address.Area, src.ServiceRequest.Address.Street, src.ServiceRequest.Address.BuildingNumber, src.ServiceRequest.Address.Latitude, src.ServiceRequest.Address.Longitude)))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ServiceRequest.ServiceRequestImages));
    }
}
