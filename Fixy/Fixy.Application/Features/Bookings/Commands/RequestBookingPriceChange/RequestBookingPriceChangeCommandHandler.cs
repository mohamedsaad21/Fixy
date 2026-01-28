using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.RequestBookingPriceChange;

public class RequestBookingPriceChangeCommandHandler : IRequestHandler<RequestBookingPriceChangeCommand, Result>
{
    private readonly IServiceBookingRepository _serviceBookingRepository;
    private readonly ICurrentUserService _currentUserService;
    public RequestBookingPriceChangeCommandHandler(IServiceBookingRepository serviceBookingRepository, ICurrentUserService currentUserService)
    {
        _serviceBookingRepository = serviceBookingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RequestBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await _serviceBookingRepository.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == request.BookingId);
        // check if booking exists or not
        if (booking == null)
            return Errors.BookingNotFound;

        var currentTechnician = await _currentUserService.GetCurrentUserAsync();
        // Ensure that technician who involved in booking is same who request price change
        if (booking.TechnicianId != currentTechnician.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.Active)
            return Errors.BookingNotActive;

        if (booking.ProposedPrice.HasValue)
            return Errors.PriceChangeAlreadyPending;

        if (request.NewProposedPrice == booking.AgreedPrice)
            return Errors.AlreadyAgreedPrice;

        booking.ProposedPrice = request.NewProposedPrice;
        booking.PriceChangeRequestedAt = DateTime.UtcNow;
        booking.Status = ServiceBookingStatus.PriceChangePendingCustomerApproval;
        await _serviceBookingRepository.UpdateAsync(booking);
        return Result.Success();
    }
}
