using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingDomainByIdToBookingByIdForCustomerResponseMapping
{
    public static GetBookingByIdForCustomerResponse ToGetBookingByIdForCustomerResponse(this ServiceBooking booking, IStringLocalizer<SharedResources> localizer)
    {
        return new GetBookingByIdForCustomerResponse
        {
            Id = booking.Id,
            Status = EnumLocalizer.Localize(booking.Status, localizer),
            AgreedPrice = booking.AgreedPrice,
            TechnicianId = booking.TechnicianId,
            TechnicianName = booking.Technician.FirstName + " " + booking.Technician.LastName,
            CreatedAt = booking.CreatedAt
        };
    }
}
