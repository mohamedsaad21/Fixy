using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<RejectBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RejectBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.PriceChangePendingCustomerApproval)
            return Errors.InvalidBookingState;

        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.InProgress;

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
