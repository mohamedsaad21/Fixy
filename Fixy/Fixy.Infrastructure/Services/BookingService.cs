using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Services;

public class BookingService(IUnitOfWork unitOfWork) : IBookingService
{
    public async Task CancelBookingAsync(ServiceBooking booking, Guid cancelledById)
    {
        booking.Status = ServiceBookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancelledById = cancelledById;

        // close chat
        var conversation = await unitOfWork.Conversations
            .GetTableAsTracking()
            .SingleOrDefaultAsync(x => x.ServiceBookingId == booking.Id);

        if (conversation != null)
            conversation.IsClosed = true;
    }
}
