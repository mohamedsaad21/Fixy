using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.Bookings.Queries.GetBookingsForCustomer;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingsDomainToGetBookingsForCustomerResponseMapping
{
    public static GetBookingsForCustomerResponse ToGetBookingsForCustomerResponse(this ServiceBooking booking, IStringLocalizer<SharedResources> localizer)
    {
        return new GetBookingsForCustomerResponse
        {
            Id = booking.Id,
            Status = EnumLocalizer.Localize(booking.Status, localizer),
            AgreedPrice = booking.AgreedPrice,
            ScheduledDateTime = booking.ScheduledDateTime,
            CreatedAt = booking.CreatedAt.ToEgyptTime()
        };
    }
}
