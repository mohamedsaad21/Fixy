using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.StartBooking;

public sealed class StartBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<StartBookingCommand, Result>
{
    public async Task<Result> Handle(StartBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.Pending)
            return Errors.InvalidBookingState;

        booking.Status = ServiceBookingStatus.InProgress;
        booking.StartedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
