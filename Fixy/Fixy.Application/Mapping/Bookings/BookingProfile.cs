using AutoMapper;

namespace Fixy.Application.Mapping.Bookings;

public partial class BookingProfile : Profile
{
    public BookingProfile()
    {
        BookingDomainByIdToBookingByIdForCustomerResponseMapping();
        BookingDomainByIdToBookingByIdForTechnicianResponseMapping();
    }
}
