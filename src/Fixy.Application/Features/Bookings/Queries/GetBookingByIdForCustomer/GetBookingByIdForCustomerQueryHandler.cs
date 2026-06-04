using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForCustomer;

public class GetBookingByIdForCustomerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, ILogger<GetBookingByIdForCustomerQueryHandler>logger) : IRequestHandler<GetBookingByIdForCustomerQuery, Result<GetBookingByIdForCustomerResponse>>
{
    public async Task<Result<GetBookingByIdForCustomerResponse>> Handle(GetBookingByIdForCustomerQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer fetching booking details. BookingId: {BookingId}", request.Id);

        var currentCustomerId = await currentUserService.GetCurrentUserId();

        var customer = await unitOfWork.Customers.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentCustomerId);

        if (customer == null)
        {   
            logger.LogWarning("Booking fetch failed — customer not found. CustomerId: {CustomerId}", currentCustomerId);
            return Errors.Unauthorized;
        }

        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.ServiceRequestImages).Include(x => x.Technician)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (booking == null)
        {
            logger.LogWarning("Booking fetch failed — booking not found. BookingId: {BookingId}, CustomerId: {CustomerId}", request.Id, currentCustomerId);
            return Errors.BookingNotFound;
        }

        var result = mapper.Map<GetBookingByIdForCustomerResponse>(booking);
        var conversation = await unitOfWork.Conversations.GetTableNoTracking().FirstOrDefaultAsync(x => x.ServiceBookingId == booking.Id && x.TechnicianId == booking.TechnicianId);
        if (conversation != null)
        {
            result.ConversationId = conversation.Id;
            logger.LogInformation("Conversation resolved for booking. BookingId: {BookingId}, ConversationId: {ConversationId}", booking.Id, conversation.Id);
        }
        else
        {
            logger.LogInformation("No conversation found for booking. BookingId: {BookingId}", booking.Id);
        }
        logger.LogInformation("Booking details fetched successfully for customer. BookingId: {BookingId}, CustomerId: {CustomerId}, BookingStatus: {BookingStatus}, HasConversation: {HasConversation}", booking.Id, currentCustomerId, booking.Status, conversation != null);
        return result;
    }
}