using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingDomainByIdToBookingByIdForTechnicianResponseMapping
{
    public static GetBookingByIdForTechnicianResponse ToGetBookingByIdForTechnicianResponse(this ServiceBooking booking)
    {
        return new GetBookingByIdForTechnicianResponse
        {
            Id = booking.Id,
            Status = booking.Status.ToString(),
            AgreedPrice = booking.AgreedPrice,
            CustomerId = booking.ServiceRequest.CustomerId,
            CustomerName = booking.ServiceRequest.Customer.FirstName + " " + booking.ServiceRequest.Customer.LastName,
            CreatedAt = booking.CreatedAt
        };
    }
}
