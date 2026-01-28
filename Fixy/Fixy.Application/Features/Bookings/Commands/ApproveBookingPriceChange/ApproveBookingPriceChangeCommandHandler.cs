using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using Fixy.Infrastructure.Persistence.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Bookings.Commands.ApproveBookingPriceChange;

public class ApproveBookingPriceChangeCommandHandler : IRequestHandler<ApproveBookingPriceChangeCommand, Result>
{
    private readonly IServiceBookingRepository _serviceBookingRepository;
    private readonly ICurrentUserService _currentUserService;
    public ApproveBookingPriceChangeCommandHandler(IServiceBookingRepository serviceBookingRepository, ICurrentUserService currentUserService)
    {
        _serviceBookingRepository = serviceBookingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ApproveBookingPriceChangeCommand request, CancellationToken cancellationToken)
    {
        var booking = await _serviceBookingRepository.GetTableAsTracking().Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        var currentCustomer = await _currentUserService.GetCurrentUserAsync();

        if (booking.ServiceRequest.Customer.Id != currentCustomer.Id)
            return Errors.Unauthorized;

        if (booking.Status != ServiceBookingStatus.PriceChangePendingCustomerApproval)
            return Errors.InvalidBookingState;

        if (booking.ProposedPrice == null)
            return Errors.NoPriceChangeToApprove;

        booking.AgreedPrice = booking.ProposedPrice.Value;
        booking.ProposedPrice = null;
        booking.Status = ServiceBookingStatus.Active;

        await _serviceBookingRepository.UpdateAsync(booking);
        return Result.Success();
    }
}
