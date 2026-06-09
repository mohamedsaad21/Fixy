using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public class SubmitTechnicianFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService, ILogger<SubmitTechnicianFeedbackCommandHandler> logger) : IRequestHandler<SubmitTechnicianFeedbackCommand, Result>
{
    public async Task<Result> Handle(SubmitTechnicianFeedbackCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician submitting feedback. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings
            .GetTableAsTracking().Include(x => x.Technician)
            .Include(x => x.ServiceRequest).FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Technician feedback submission failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingFeedback && booking.Status != ServiceBookingStatus.CustomerCompleted)
        {
            logger.LogWarning("Technician feedback submission failed — invalid booking status. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingStatus;
        }

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (currentTechnician == null)
        {
            logger.LogWarning("Technician feedback submission failed — no current user resolved. BookingId: {BookingId}", request.BookingId);
            return Errors.Unauthorized;
        }

        var isExists = await unitOfWork.TechnicianFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (isExists)
        {
            logger.LogWarning("Technician feedback submission failed — feedback already submitted. BookingId: {BookingId}, TechnicianId: {TechnicianId}", request.BookingId, currentTechnician.Id);
            return Errors.FeebackAlreadySubmitted;
        }

        var feedback = request.ToTechnicianFeedbackDomain(booking.ServiceRequest.CustomerId, currentTechnician.Id);

        await unitOfWork.TechnicianFeedbacks.AddAsync(feedback);
        var IsCustomerFeedbackExists = await unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);
        if (IsCustomerFeedbackExists)
        {
            booking.Status = ServiceBookingStatus.FullCompleted;
        }
        else
        {
            booking.Status = ServiceBookingStatus.TechnicianCompleted;
        }
        booking.Technician.CompletedBookings += 1;
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Technician feedback saved. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}", booking.Id, currentTechnician.Id, booking.ServiceRequest.CustomerId);

        if (!booking.IsEvaluated && booking.Status == ServiceBookingStatus.FullCompleted)
        {
            BackgroundJob.Enqueue<IRatingService>(x => x.PredictTechnicianRatingAsync(booking.Id));
        }

        logger.LogInformation("Technician feedback submitted successfully. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, NewBookingStatus: {NewBookingStatus}, TechnicianCompletedBookings: {TechnicianCompletedBookings}",
            booking.Id, currentTechnician.Id, booking.ServiceRequest.CustomerId,
            booking.Status, booking.Technician.CompletedBookings);

        logger.LogInformation("Technician feedback submitted successfully. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, NewBookingStatus: {NewBookingStatus}, TechnicianCompletedBookings: {TechnicianCompletedBookings}",
            booking.Id, currentTechnician.Id, booking.ServiceRequest.CustomerId,
            booking.Status, booking.Technician.CompletedBookings);

        return Result.Success();
    }
}
