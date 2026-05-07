using Fixy.Application.Common.Helpers;
using Fixy.Application.Features.Bookings.Queries.GetBookingsForTechnician;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Helpers;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Mapping.Bookings.Queries;

public static class BookingsDomainToGetBookingsForTechnicianResponseMapping
{
    public static GetBookingsForTechnicianResponse ToGetBookingsForTechnicianResponse(this ServiceBooking booking, IStringLocalizer<SharedResources> localizer)
    {
        return new GetBookingsForTechnicianResponse
        {
            Id = booking.Id,
            Status = EnumLocalizer.Localize(booking.Status, localizer),
            AgreedPrice = booking.AgreedPrice,
            CreatedAt = booking.CreatedAt.ToEgyptTime()
        };
    }
}
