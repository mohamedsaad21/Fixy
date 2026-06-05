using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Feedbacks.Queries.GetPendingTechnicianFeedbackStatus;

public sealed class GetPendingTechnicianFeedbackStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService, ILogger<GetPendingTechnicianFeedbackStatusQueryHandler> logger) : IRequestHandler<GetPendingTechnicianFeedbackStatusQuery, Result<GetPendingTechnicianFeedbackStatusResponse>>
{
    public async Task<Result<GetPendingTechnicianFeedbackStatusResponse>> Handle(GetPendingTechnicianFeedbackStatusQuery request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        logger.LogInformation("Checking pending technician feedback status. TechnicianId: {TechnicianId}", currentTechnicianId);
                
        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Pending technician feedback status check failed — technician not found. TechnicianId: {TechnicianId}", currentTechnicianId);
            return Errors.Unauthorized;
        }

        var pendingTechnicianFeedbackBookingId = await feedbackService.GetPendingTechnicianFeedbackBookingIdAsync(technician.Id);
        var hasPendingFeedback = pendingTechnicianFeedbackBookingId != null;

        if (hasPendingFeedback)
            logger.LogInformation("Pending feedback found for technician — bid submissions are locked. TechnicianId: {TechnicianId}, PendingBookingId: {PendingBookingId}", technician.Id, pendingTechnicianFeedbackBookingId);
        else
            logger.LogInformation("No pending feedback for technician — bid submissions are unlocked. TechnicianId: {TechnicianId}", technician.Id);

        return new GetPendingTechnicianFeedbackStatusResponse
        {
            HasPendingFeedback = hasPendingFeedback,
            BookingId = pendingTechnicianFeedbackBookingId
        };
    }
}
