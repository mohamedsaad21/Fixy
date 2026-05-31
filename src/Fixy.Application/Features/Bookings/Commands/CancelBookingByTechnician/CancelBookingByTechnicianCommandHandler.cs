using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Bookings.Commands.CancelBookingByTechnician;

public sealed class CancelBookingByTechnicianCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBookingService bookingService, INotificationService notificationService) : IRequestHandler<CancelBookingByTechnicianCommand, Result>
{
    public async Task<Result> Handle(CancelBookingByTechnicianCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Technician attempting to cancel booking. BookingId: {BookingId}", request.BookingId);

        var currentTechnicianId = await currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
        {
            Log.Warning("Booking cancellation failed — technician not found. TechnicianId: {TechnicianId}", currentTechnicianId);
            return Errors.Unauthorized;
        }

        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            Log.Warning("Booking cancellation failed — booking not found. BookingId: {BookingId}, TechnicianId: {TechnicianId}", request.BookingId, technician.Id);
            return Errors.BookingNotFound;
        }

        if (booking.TechnicianId != technician.Id)
        {
            Log.Warning("Booking cancellation failed — technician not assigned to this booking. BookingId: {BookingId}, BookingTechnicianId: {BookingTechnicianId}, RequestingTechnicianId: {RequestingTechnicianId}", request.BookingId, booking.TechnicianId, technician.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.InProgress)
        {
            Log.Warning("Booking cancellation failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.CannotCancelAtThisStage;
        }
        var previousCancellationRate = booking.TechnicianCancellationReason;
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

        Log.Information("Booking cancelled by technician successfully. BookingId: {BookingId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}, Reason: {Reason}, PreviousCancellationRate: {PreviousCancellationRate}, NewCancellationRate: {NewCancellationRate}",
            request.BookingId, technician.Id, booking.ServiceRequest.Customer.Id, request.Reason,
            previousCancellationRate, technician.CancellationRate);

        return Result.Success();
    }
}