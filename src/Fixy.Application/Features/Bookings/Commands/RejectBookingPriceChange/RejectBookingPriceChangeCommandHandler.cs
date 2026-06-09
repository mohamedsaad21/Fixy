using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.RejectBookingPriceChange;

public class RejectBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<RejectBookingPriceChangeCommandHandler>logger) : IRequestHandler<RejectBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(RejectBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to reject price change. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician).Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Price change rejection failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            logger.LogWarning("Price change rejection failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, booking.ServiceRequest.Customer.Id, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
        {
            logger.LogWarning("Price change rejection failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingState;
        }
        var rejectedProposedPrice = booking.ProposedPrice;
        booking.Status = ServiceBookingStatus.InProgress;        

        var technician = booking.Technician;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Price change rejected by customer — booking resumed at original price. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, RejectedProposedPrice: {RejectedProposedPrice}, ResumedAtPrice: {ResumedAtPrice}",
            request.BookingId, currentCustomer.Id, booking.Technician.Id,
            rejectedProposedPrice, booking.AgreedPrice);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician,
            NotificationType.PriceChangeRejected,
            SharedResourcesKeys.NotificationPriceChangeRejectedTitle,
            SharedResourcesKeys.NotificationPriceChangeRejectedBody
        ));

        return Result.Success();
    }
}