using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public class ApproveBookingPriceChangeCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<ApproveBookingPriceChangeCommandHandler>logger) : IRequestHandler<ApproveBookingPriceChangeCommand, Result>
{
    public async Task<Result> Handle(ApproveBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to approve price change. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking()
            .Include(x => x.Technician)
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Price change approval failed — booking not found. BookingId: {BookingId}", request.BookingId);            
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            logger.LogWarning("Price change approval failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingUserId: {RequestingUserId}", request.BookingId, booking.ServiceRequest.Customer.Id, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingPriceChangeApproval)
        {
            logger.LogWarning("Price change approval failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingState;
        }

        if (booking.ProposedPrice == null)
        {
            logger.LogWarning("Price change approval failed — no proposed price found. BookingId: {BookingId}", request.BookingId);
            return Errors.NoPriceChangeToApprove;
        }
        var previousPrice = booking.AgreedPrice;
        booking.AgreedPrice = booking.ProposedPrice.Value;
        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.InProgress;

        var technician = booking.Technician;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Price change approved successfully. BookingId: {BookingId}, CustomerId: {CustomerId}, TechnicianId: {TechnicianId}, PreviousPrice: {PreviousPrice}, ApprovedPrice: {ApprovedPrice}", request.BookingId, currentCustomer.Id, booking.Technician.Id, previousPrice, booking.AgreedPrice);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            technician,
            NotificationType.PriceChangeApproved,
            SharedResourcesKeys.NotificationPriceChangeApprovedTitle,
            SharedResourcesKeys.NotificationPriceChangeApprovedBody
        ));

        return Result.Success();
    }
}
