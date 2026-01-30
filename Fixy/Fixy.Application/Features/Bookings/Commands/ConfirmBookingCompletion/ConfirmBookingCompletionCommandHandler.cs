using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;

public class ConfirmBookingCompletionCommandHandler : IRequestHandler<ConfirmBookingCompletionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    public ConfirmBookingCompletionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }
    public async Task<Result> Handle(ConfirmBookingCompletionCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await _currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.CompletedPendingCustomerConfirmation)
            return Errors.InvalidBookingState;

        booking.Status = ServiceBookingStatus.Completed;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
