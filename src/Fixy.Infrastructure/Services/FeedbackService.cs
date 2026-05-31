using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IUnitOfWork _unitOfWork;
    public FeedbackService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessFeedbackCompletionAsync(ServiceBooking booking)
    {
        var hasCustomerFeedback = await _unitOfWork.CustomerFeedbacks
            .GetTableNoTracking()
            .AnyAsync(x => x.ServiceBookingId == booking.Id);

        var hasTechnicianFeedback = await _unitOfWork.TechnicianFeedbacks
            .GetTableNoTracking()
            .AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (hasCustomerFeedback && hasTechnicianFeedback)
        {
            if (booking.IsEvaluated) return;
            // Fire and Forget
            BackgroundJob.Enqueue<IRatingService>(x => x.PredictTechnicianRatingAsync(booking.Id));
        }
    }

    public async Task<Guid?> GetPendingCustomerFeedbackBookingIdAsync(Guid customerId)
    {
        var lastUnCompletedBooking = await _unitOfWork.Bookings.GetTableNoTracking()
            .SingleOrDefaultAsync(x => x.ServiceRequest.CustomerId == customerId && (x.Status == ServiceBookingStatus.TechnicianCompleted || x.Status == ServiceBookingStatus.AwaitingFeedback));

        return lastUnCompletedBooking != null? lastUnCompletedBooking.Id : null;
    }

    public async Task<Guid?> GetPendingTechnicianFeedbackBookingIdAsync(Guid technicianId)
    {
        var lastUnCompletedBooking = await _unitOfWork.Bookings.GetTableNoTracking()
            .SingleOrDefaultAsync(x => x.TechnicianId == technicianId && (x.Status == ServiceBookingStatus.CustomerCompleted || x.Status == ServiceBookingStatus.AwaitingFeedback));

        return lastUnCompletedBooking != null ? lastUnCompletedBooking.Id : null;
    }
}