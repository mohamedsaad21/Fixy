using Fixy.Application.Features.Bookings.Queries.GetBookingById;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingDomainByIdToBookingByIdDtoMapping
{
    public static GetBookingByIdDto ToBookingDto(this ServiceBooking booking)
    {
        return new GetBookingByIdDto
            (
                booking.Id,
                booking.Status.ToString(),
                booking.AgreedPrice, 
                booking.CreatedAt            
            );
    }
}
