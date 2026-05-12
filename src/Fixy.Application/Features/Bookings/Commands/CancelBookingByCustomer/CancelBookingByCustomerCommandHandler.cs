using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByCustomer;

public sealed class CancelBookingByCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
    IBookingService bookingService, INotificationService notificationService, IStringLocalizer<SharedResources> localizer) : IRequestHandler<CancelBookingByCustomerCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await currentUserService.GetCurrentUserAsync();

        if (customer == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer).Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.ServiceRequest.CustomerId != customer.Id)
            return Errors.Unauthorized;

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.CannotCancelAtThisStage;

        booking.ServiceRequest.Customer.CancelledBookings += 1;        
        customer.CancellationRate = customer.TotalBookings > 0? (double)customer.CancelledBookings / customer.TotalBookings * 100 : 0;
        booking.CustomerCancellationReason = request.Reason;
        booking.CancellationNote = request.Note;

        await bookingService.CancelBookingByCustomerAsync(booking, customer.Id, request.ReopenServiceRequest);
        
        // send notification to technician
        var technician = booking.Technician;

        await notificationService.SendFullNotificationAsync(
            technician,
            NotificationType.BookingCancelledByCustomer,
            SharedResourcesKeys.NotificationBookingCancelledByCustomerTitle,
            SharedResourcesKeys.NotificationBookingCancelledByCustomerBody
        );
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
