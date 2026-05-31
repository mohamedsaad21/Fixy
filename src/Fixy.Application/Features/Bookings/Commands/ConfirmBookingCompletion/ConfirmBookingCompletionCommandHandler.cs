using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Bookings.Commands.ConfirmBookingCompletion;

public class ConfirmBookingCompletionCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<ConfirmBookingCompletionCommand, Result>
{
    public async Task<Result> Handle(ConfirmBookingCompletionCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Customer attempting to confirm booking completion. BookingId: {BookingId}", request.BookingId);
        
        var booking = await unitOfWork.Bookings.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            Log.Warning("Booking completion confirmation failed — booking not found. BookingId: {BookingId}", request.BookingId);
            return Errors.BookingNotFound;
        }

        var currentCustomer = await currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
        {
            Log.Warning("Booking completion confirmation failed — unauthorized customer. BookingId: {BookingId}, BookingCustomerId: {BookingCustomerId}, RequestingCustomerId: {RequestingCustomerId}", request.BookingId, booking.ServiceRequest.Customer.Id, currentCustomer.Id);
            return Errors.Unauthorized;
        }

        if (booking.Status != ServiceBookingStatus.AwaitingCustomerConfirmationForCompletion)
        {
            Log.Warning("Booking completion confirmation failed — invalid booking state. BookingId: {BookingId}, CurrentStatus: {CurrentStatus}", request.BookingId, booking.Status);
            return Errors.InvalidBookingState;
        }

        booking.Status = ServiceBookingStatus.AwaitingPayment;
        booking.IsCustomerConfirmed = true;
        booking.CustomerConfirmedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        Log.Information("Booking completion confirmed by customer — awaiting payment. BookingId: {BookingId}, CustomerId: {CustomerId}, AgreedPrice: {AgreedPrice}, ConfirmedAt: {ConfirmedAt}",
            request.BookingId, currentCustomer.Id, booking.AgreedPrice, booking.CustomerConfirmedAt);

        return Result.Success();
    }
}