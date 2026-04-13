using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public class SubmitTechnicianFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<SubmitTechnicianFeedbackCommand, Result>
{
    public async Task<Result> Handle(SubmitTechnicianFeedbackCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest).FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.Status != ServiceBookingStatus.Completed)
            return Errors.BookingNotCompleted;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();

        var IsExists = await unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (IsExists)
            return Errors.FeebackAlreadySubmitted;

        var feedback = request.ToTechnicianFeedbackDomain(booking.ServiceRequest.CustomerId, currentTechnician.Id);

        await unitOfWork.TechnicianFeedbacks.AddAsync(feedback);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
