using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;

public class ConfirmBookingCompletionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<ConfirmBookingCompletionCommand, Result>
{
    public async Task<Result> Handle(ConfirmBookingCompletionCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.CompletedPendingCustomerConfirmation)
            return Errors.InvalidBookingState;

        booking.Status = ServiceBookingStatus.PaymentPending;

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
