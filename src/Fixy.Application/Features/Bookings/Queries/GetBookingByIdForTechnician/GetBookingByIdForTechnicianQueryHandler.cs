using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Entities;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Fixy.Application.Features.Bookings.Queries.GetBookingByIdForTechnician;

public sealed class GetBookingByIdForTechnicianQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, ILogger<GetBookingByIdForTechnicianQueryHandler> logger) : IRequestHandler<GetBookingByIdForTechnicianQuery, Result<GetBookingByIdForTechnicianResponse>>
{
    public async Task<Result<GetBookingByIdForTechnicianResponse>> Handle(GetBookingByIdForTechnicianQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Technician fetching booking details. BookingId: {BookingId}", request.Id);

        var currentUser = await currentUserService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            logger.LogWarning("Booking fetch failed — no current user resolved. BookingId: {BookingId}", request.Id);
            return Errors.Unauthorized;
        }

        var technician = await unitOfWork.Technicians.GetTableNoTracking().FirstOrDefaultAsync(x => x.Id == currentUser.Id);
        if (technician == null)
        {
            logger.LogWarning("Booking fetch failed — current user is not a technician. UserId: {UserId}", currentUser.Id);
            return Errors.TechnicianNotFound;
        }

        var booking = await unitOfWork.Bookings.GetTableNoTracking()
            .Include(x => x.ServiceRequest).ThenInclude(x => x.Customer)
            .Include(x => x.ServiceRequest).ThenInclude(x => x.ServiceRequestImages)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.TechnicianId == technician.Id);

        if (booking == null)
        {
            logger.LogWarning("Booking fetch failed — booking not found or not assigned to technician. BookingId: {BookingId}, TechnicianId: {TechnicianId}", request.Id, technician.Id);
            return Errors.BookingNotFound;
        }

        var result = mapper.Map<GetBookingByIdForTechnicianResponse>(booking);
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

        logger.LogInformation("Booking details fetched successfully for technician. BookingId: {BookingId}, TechnicianId: {TechnicianId}, BookingStatus: {BookingStatus}, HasConversation: {HasConversation}", booking.Id, technician.Id, booking.Status, conversation != null);
        return result;
    }
}