using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingDomainByIdToBookingByIdForTechnicianResponseMapping
{
    public static GetBookingByIdForTechnicianResponse ToGetBookingByIdForTechnicianResponse(this ServiceBooking booking, IStringLocalizer<SharedResources> localizer)
    {
        return new GetBookingByIdForTechnicianResponse
        {
            Id = booking.Id,
            Status = EnumLocalizer.Localize(booking.Status, localizer),
            AgreedPrice = booking.AgreedPrice,
            CustomerId = booking.ServiceRequest.CustomerId,
            CustomerName = booking.ServiceRequest.Customer.FirstName + " " + booking.ServiceRequest.Customer.LastName,
            CreatedAt = booking.CreatedAt
        };
    }
}
