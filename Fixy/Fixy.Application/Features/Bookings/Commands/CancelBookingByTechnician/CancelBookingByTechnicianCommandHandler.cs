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
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.TechnicianId != technician.Id)
            return Errors.Unauthorized;

        if(booking.Status != ServiceBookingStatus.InProgress)
            return Errors.CannotCancelAtThisStage;

        technician.CancelledBookings += 1;
        technician.CancellationRate = technician.TotalBookings > 0? (double)technician.CancelledBookings / technician.TotalBookings * 100 : 0;
        booking.TechnicianCancellationReason = request.Reason;
        booking.CancellationNote = request.Notes;

        await bookingService.CancelBookingByTechnicianAsync(booking, technician);
        
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
