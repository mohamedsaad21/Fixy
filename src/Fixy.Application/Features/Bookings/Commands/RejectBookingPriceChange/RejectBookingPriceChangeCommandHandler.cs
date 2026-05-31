using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<RejectBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RejectBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Customer attempting to reject price change. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            Log.Warning("Price change rejection failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            Log.Warning("Price change rejection failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, booking.ServiceRequest.Customer.Id, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
        {
            Log.Warning("Price change rejection failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingState;
        }
        var rejectedProposedPrice = booking.ProposedPrice;
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

        Log.Information("Price change rejected by customer — booking resumed at original price. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, RejectedProposedPrice: {RejectedProposedPrice}, ResumedAtPrice: {ResumedAtPrice}",
            request.BookingId, currentCustomer.Id, booking.Technician.Id,
            rejectedProposedPrice, booking.AgreedPrice);

        return Result.Success();
    }
}