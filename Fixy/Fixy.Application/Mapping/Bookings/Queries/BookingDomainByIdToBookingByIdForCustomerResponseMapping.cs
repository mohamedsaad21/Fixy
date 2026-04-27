using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingDomainByIdToBookingByIdForCustomerResponseMapping
{
    public static GetBookingByIdForCustomerResponse ToGetBookingByIdForCustomerResponse(this ServiceBooking booking)
    {
        return new GetBookingByIdForCustomerResponse
        {
            Id = booking.Id,
            Status = booking.Status.ToString(),
            AgreedPrice = booking.AgreedPrice,
            TechnicianId = booking.TechnicianId,
            TechnicianName = booking.Technician.FirstName + " " + booking.Technician.LastName,
            CreatedAt = booking.CreatedAt
        };
    }
}
