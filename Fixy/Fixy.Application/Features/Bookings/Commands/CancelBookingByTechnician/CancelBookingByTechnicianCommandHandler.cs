using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;

public sealed class CancelBookingByTechnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBookingService bookingService, INotificationService notificationService) : IRequestHandler<CancelBookingByTechnicianCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByTechnicianCommand request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);
        
        if (technician == null)
            return Errors.Unauthorized;

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(X => X.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.TechnicianId != technician.Id)
            return Errors.Unauthorized;

        if (booking.Status == ServiceBookingStatus.Cancelled)
            return Errors.AlreadyCancelled;

        if(booking.Status != ServiceBookingStatus.InProgress && booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
            return Errors.CannotCancelAtThisStage;

        booking.TechnicianCancellationReason = request.Reason;
        booking.CancellationNote = request.Notes;
        technician.CancelledBookings += 1;

        if (technician.TotalBookings > 0)
            technician.CancellationRate = (double)technician.CancelledBookings / technician.TotalBookings * 100;
        else
            technician.CancellationRate = 0;

        await bookingService.CancelBookingAsync(booking, technician.Id);
        // send notification to other user
        var customer = booking.ServiceRequest.Customer;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.BookingCancelledByTechnician,
            SharedResourcesKeys.NotificationBookingCancelledByTechnicianTitle,
            SharedResourcesKeys.NotificationBookingCancelledByTechnicianBody
        );
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
