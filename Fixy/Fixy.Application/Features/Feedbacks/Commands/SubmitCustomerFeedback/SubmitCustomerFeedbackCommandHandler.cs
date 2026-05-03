using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;

public class SubmitCustomerFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService) : IRequestHandler<SubmitCustomerFeedbackCommand, Result>
{
    public async Task<Result> Handle(SubmitCustomerFeedbackCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.Status != ServiceBookingStatus.Completed)
            return Errors.BookingNotCompleted;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        var IsExists = await unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (IsExists)
            return Errors.FeebackAlreadySubmitted;

        var feedback = request.ToCustomerFeedbackDomain(currentCustomer.Id, booking.TechnicianId);

        await unitOfWork.CustomerFeedbacks.AddAsync(feedback);

        await unitOfWork.SaveChangesAsync();

        await feedbackService.ProcessFeedbackCompletionAsync(booking);

        return Result.Success();
    }
}
