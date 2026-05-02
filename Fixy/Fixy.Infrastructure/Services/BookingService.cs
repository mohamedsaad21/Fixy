using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Services;

public class BookingService(IUnitOfWork unitOfWork) : IBookingService
{
    public async Task CancelBookingByTechnicianAsync(ServiceBooking booking, Technician technician)
    {
        booking.Status = ServiceBookingStatus.CancelledByTechnician;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancelledById = technician.Id;

        // close chat
        await unitOfWork.Conversations.CloseConversationAsync(booking.Id);

        await ReopenServiceRequestAsync(booking.ServiceRequest, technician);
    }

    public async Task CancelBookingByCustomerAsync(ServiceBooking booking, Guid customerId, bool reopenServiceRequest)
    {
        booking.Status = ServiceBookingStatus.CancelledByCustomer;
        booking.CancelledAt = DateTime.UtcNow;
        booking.CancelledById = customerId;

        // close chat
        await unitOfWork.Conversations.CloseConversationAsync(booking.Id);

        if (reopenServiceRequest)
            await ReopenServiceRequestAsync(booking.ServiceRequest, booking.Technician);
        else
            booking.ServiceRequest.Status = ServiceRequestStatus.Cancelled;
    }

    private async Task ReopenServiceRequestAsync(ServiceRequest serviceRequest, Technician blockedTechnician)
    {
        serviceRequest.Status = ServiceRequestStatus.Pending;
        serviceRequest.HadPreviousCancellation = true;
        serviceRequest.UpdatedAt = DateTime.UtcNow;

        var blockedServiceRequest = new BlockedServiceRequest
        {
            TechnicianId = blockedTechnician.Id,
            ServiceRequestId = serviceRequest.Id
        };
        await unitOfWork.BlockedServiceRequests.AddAsync(blockedServiceRequest);

        var priceOffer = await unitOfWork.PriceOffers.GetTableAsTracking()
            .FirstOrDefaultAsync(x => x.ServiceRequestId == serviceRequest.Id && x.TechnicianId == blockedTechnician.Id);

        if (priceOffer != null)
        {
            priceOffer.IsDeleted = true;
            priceOffer.DeletedAt = DateTime.UtcNow;
        }
        await ExpireStaleOffers(serviceRequest);
    }

    private async Task ExpireStaleOffers(ServiceRequest serviceRequest)
    {
        if (serviceRequest.ScheduledDateTime >= DateTime.UtcNow)
            return;

        foreach (var offer in serviceRequest.PriceOffers)
        {
            offer.IsDeleted = true;
            offer.DeletedAt = DateTime.UtcNow;
        }
    }
}
