using AutoMapper;

namespace Fixy.Application.Mapping.Admin;

public partial class AdminProfile : Profile
{
    public AdminProfile()
    {
        BookingDomainByIdToGetBookingByIdResponseMapping();
    }
}
