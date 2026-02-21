using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitTechnicianFeedback;

public class SubmitTechnicianFeedbackCommandHandler : IRequestHandler<SubmitTechnicianFeedbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmitTechnicianFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SubmitTechnicianFeedbackCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetTableNoTracking().Include(x => x.ServiceRequest).FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.Status != ServiceBookingStatus.Completed)
            return Errors.BookingNotCompleted;

        var currentTechnician = await _currentUserService.GetCurrentUserAsync();

        var IsExists = await _unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (IsExists)
            return Errors.FeebackAlreadySubmitted;

        var feedback = request.ToTechnicianFeedbackDomain(booking.ServiceRequest.CustomerId, currentTechnician.Id);

        await _unitOfWork.TechnicianFeedbacks.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
