using Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingsDomainToGetBookingsForCustomerResponseMapping
{
    public static GetBookingsForCustomerResponse ToGetBookingsForCustomerResponse(this ServiceBooking booking)
    {
        return new GetBookingsForCustomerResponse
        {
            Id = booking.Id,
            Status = booking.Status.ToString(),
            AgreedPrice = booking.AgreedPrice,
            CreatedAt = booking.CreatedAt
        };
    }
}
