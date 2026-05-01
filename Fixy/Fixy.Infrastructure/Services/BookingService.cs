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

        await ReopenServiceRequest(booking.ServiceRequest, booking.Technician);
    }

    private async Task<bool> ReopenServiceRequest(ServiceRequest serviceRequest, Technician technician)
    {
        serviceRequest.Status = ServiceRequestStatus.Pending;
        var blockedServiceRequest = new BlockedServiceRequest
        {
            TechnicianId = technician.Id,
            ServiceRequestId = serviceRequest.Id
        };
        await unitOfWork.BlockedServiceRequests.AddAsync(blockedServiceRequest);
        var priceOffer = await unitOfWork.PriceOffers.GetTableAsTracking()
            .FirstOrDefaultAsync(x => x.ServiceRequestId == serviceRequest.Id && x.TechnicianId == technician.Id);

        if (priceOffer == null)
            return false;

        priceOffer.IsDeleted = true;
        priceOffer.DeletedAt = DateTime.UtcNow;
        return true;
    }
}
