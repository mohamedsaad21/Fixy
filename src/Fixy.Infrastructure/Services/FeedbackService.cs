using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    public FeedbackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid?> GetPendingCustomerFeedbackBookingIdAsync(Guid customerId)
    {
        var lastUnCompletedBooking = await _unitOfWork.Bookings.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceRequest.CustomerId == customerId && (x.Status == ServiceBookingStatus.TechnicianCompleted || x.Status == ServiceBookingStatus.AwaitingFeedback));

        return lastUnCompletedBooking != null? lastUnCompletedBooking.Id : null;
    }

    public async Task<Guid?> GetPendingTechnicianFeedbackBookingIdAsync(Guid technicianId)
    {
        var lastUnCompletedBooking = await _unitOfWork.Bookings.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.TechnicianId == technicianId && (x.Status == ServiceBookingStatus.CustomerCompleted || x.Status == ServiceBookingStatus.AwaitingFeedback));

        return lastUnCompletedBooking != null ? lastUnCompletedBooking.Id : null;
    }
}