using Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingsDomainToGetBookingsForTechnicianResponseMapping
{
    public static GetBookingsForTechnicianResponse ToGetBookingsForTechnicianResponse(this ServiceBooking booking)
    {
        return new GetBookingsForTechnicianResponse
        {
            Id = booking.Id,
            Status = booking.Status.ToString(),
            AgreedPrice = booking.AgreedPrice,
            CreatedAt = booking.CreatedAt
        };
    }
}
