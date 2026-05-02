using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
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

    public async Task ProcessFeedbackCompletionAsync(ServiceBooking Booking)
    {
        var CustomerFeedback = await _unitOfWork.CustomerFeedbacks
            .GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceBookingId == Booking.Id);

        var TechnicianFeedback = await _unitOfWork.TechnicianFeedbacks
            .GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.ServiceBookingId == Booking.Id);

        if (CustomerFeedback != null && TechnicianFeedback != null)
        {
            if (Booking.IsEvaluated) return;
            // Fire and Forget
            var jobId = BackgroundJob.Enqueue<IRatingService>
                (x => x.UpdateTechnicianRatingAsync(Booking, CustomerFeedback, TechnicianFeedback));
        }
}
}