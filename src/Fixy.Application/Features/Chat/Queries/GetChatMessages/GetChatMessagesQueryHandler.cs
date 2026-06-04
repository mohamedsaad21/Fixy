using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Chat.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Chat.Queries.GetChatMessages;

public sealed class GetChatMessagesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<GetChatMessagesQueryHandler>logger) : IRequestHandler<GetChatMessagesQuery, Result<PaginatedResult<GetChatMessagesResponse>>>
{
    public async Task<Result<PaginatedResult<GetChatMessagesResponse>>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching chat messages. BookingId: {BookingId}, Page: {PageNumber}, PageSize: {PageSize}", request.BookingId, request.PageNumber, request.PageSize);

        var user = await currentUserService.GetCurrentUserAsync();

        if (user == null)
        {
            logger.LogWarning("Chat messages fetch failed — no current user resolved. BookingId: {BookingId}", request.BookingId);
            return Errors.Unauthorized;
        }
        
        var booking = await unitOfWork.Bookings
            .GetTableNoTracking()
            .Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Chat messages fetch failed — booking not found. BookingId: {BookingId}, UserId: {UserId}", request.BookingId, user.Id);
            return Errors.BookingNotFound;
        }

        if (booking.TechnicianId != user.Id && booking.ServiceRequest.Customer.Id != user.Id)
        {
            logger.LogWarning("Chat messages fetch failed — user is not a participant in this booking. BookingId: {BookingId}, UserId: {UserId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}", request.BookingId, user.Id, booking.TechnicianId, booking.ServiceRequest.Customer.Id);
            return Errors.Unauthorized;
        }

        var query = unitOfWork.ChatMessages.GetTableNoTracking().Include(x => x.Conversation)
            .Where(x => x.Conversation.ServiceBookingId == request.BookingId)
            .OrderByDescending(x => x.SentAt);

        var paginated = await query.Select(x => x.ToChatMessageResponse())
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
        logger.LogInformation("Chat messages fetched successfully. BookingId: {BookingId}, UserId: {UserId}, TotalCount: {TotalCount}, Page: {PageNumber}, PageSize: {PageSize}",
            request.BookingId, user.Id, paginated.TotalCount, paginated.CurrentPage, paginated.PageSize);
        return paginated;
    }
}
