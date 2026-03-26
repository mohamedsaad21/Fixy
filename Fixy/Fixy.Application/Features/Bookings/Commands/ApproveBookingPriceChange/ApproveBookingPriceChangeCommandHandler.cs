using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public class ApproveBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<ApproveBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(ApproveBookingPriceChangeCommand request, CancellationToken cancellationToken)
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

        if (booking.ProposedPrice == null)
            return Errors.NoPriceChangeToApprove;

        booking.AgreedPrice = booking.ProposedPrice.Value;
        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.Active;

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
