using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public class SubmitTechnicianFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IFeedbackService feedbackService) : IRequestHandler<SubmitTechnicianFeedbackCommand, Result>
{
    public async Task<Result> Handle(SubmitTechnicianFeedbackCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings
            .GetTableAsTracking().Include(x => x.ServiceRequest).FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.Status != ServiceBookingStatus.Completed)
            return Errors.BookingNotCompleted;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();

        var isExists = await unitOfWork.TechnicianFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (isExists)
            return Errors.FeebackAlreadySubmitted;

        var feedback = request.ToTechnicianFeedbackDomain(booking.ServiceRequest.CustomerId, currentTechnician.Id);

        await unitOfWork.TechnicianFeedbacks.AddAsync(feedback);

        await unitOfWork.SaveChangesAsync();

        await feedbackService.ProcessFeedbackCompletionAsync(booking);

        return Result.Success();
    }
}
