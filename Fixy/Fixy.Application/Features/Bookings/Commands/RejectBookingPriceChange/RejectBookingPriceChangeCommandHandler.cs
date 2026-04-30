using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<RejectBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RejectBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.InvalidBookingState;

        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.InProgress;

        var technician = booking.Technician;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.PriceChangeRejected,
            SharedResourcesKeys.NotificationPriceChangeRejectedTitle,
            SharedResourcesKeys.NotificationPriceChangeRejectedBody
        );
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
