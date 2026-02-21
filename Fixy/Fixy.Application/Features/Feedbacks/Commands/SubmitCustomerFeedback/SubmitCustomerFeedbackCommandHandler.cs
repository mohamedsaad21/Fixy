using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Application.Mapping.Feedbacks.Commands;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Feedbacks.Commands.SubmitCustomerFeedback;

public class SubmitCustomerFeedbackCommandHandler : IRequestHandler<SubmitCustomerFeedbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmitCustomerFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SubmitCustomerFeedbackCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.Status != ServiceBookingStatus.Completed)
            return Errors.BookingNotCompleted;

        var currentCustomer = await _currentUserService.GetCurrentUserAsync();

        var IsExists = await _unitOfWork.CustomerFeedbacks.GetTableNoTracking().AnyAsync(x => x.ServiceBookingId == booking.Id);

        if (IsExists)
            return Errors.FeebackAlreadySubmitted;

        var feedback = request.ToCustomerFeedbackDomain(currentCustomer.Id, booking.TechnicianId);

        await _unitOfWork.CustomerFeedbacks.AddAsync(feedback);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
