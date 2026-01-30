using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.MarkBookingCompleted;

public class MarkBookingCompletedCommandHandler : IRequestHandler<MarkBookingCompletedCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkBookingCompletedCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(MarkBookingCompletedCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.Active)
            return Errors.BookingNotActive;

        booking.Status = ServiceBookingStatus.CompletedPendingCustomerConfirmation;
        booking.CompletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
