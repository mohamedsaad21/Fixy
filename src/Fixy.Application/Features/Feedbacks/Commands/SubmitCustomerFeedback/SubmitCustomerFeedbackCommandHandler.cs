using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;

public class SubmitCustomerFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService, ILogger<SubmitCustomerFeedbackCommandHandler> logger) : IRequestHandler<SubmitCustomerFeedbackCommand, Result>
{
    public async Task<Result> Handle(SubmitCustomerFeedbackCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer submitting feedback. BookingId: {BookingId}", request.BookingId);
                
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Customer feedback submission failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingFeedback && booking.Status != ServiceBookingStatus.TechnicianCompleted)
        {
            logger.LogWarning("Customer feedback submission failed — invalid booking status. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingStatus;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (currentCustomer == null)
        {
            logger.LogWarning("Customer feedback submission failed — no current user resolved. BookingId: {BookingId}", request.BookingId);
            return Errors.Unauthorized;
        }

        var IsExists = await unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (IsExists)
        {
            logger.LogWarning("Customer feedback submission failed — feedback already submitted. BookingId: {BookingId}, CustomerId: {CustomerId}", request.BookingId, currentCustomer.Id);
            return Errors.FeebackAlreadySubmitted;
        }

        var feedback = request.ToCustomerFeedbackDomain(currentCustomer.Id, booking.TechnicianId);

        await unitOfWork.CustomerFeedbacks.AddAsync(feedback);
        booking.Status = ServiceBookingStatus.CustomerCompleted;
        booking.ServiceRequest.Customer.CompletedBookings += 1;
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Customer feedback saved. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}", booking.Id, currentCustomer.Id, booking.TechnicianId);

        await feedbackService.ProcessFeedbackCompletionAsync(booking);

        logger.LogInformation("Customer feedback submitted successfully. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, NewBookingStatus: {NewBookingStatus}, CustomerCompletedBookings: {CustomerCompletedBookings}",
            booking.Id, currentCustomer.Id, booking.TechnicianId,
            booking.Status, booking.ServiceRequest.Customer.CompletedBookings);
        return Result.Success();
    }
}
