using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.Chat.Queries;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Chat.Queries.GetChatMessages;

public sealed class GetChatMessagesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetChatMessagesQuery, Result<PaginatedResult<GetChatMessagesResponse>>>
{
    public async Task<Result<PaginatedResult<GetChatMessagesResponse>>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        var booking = await unitOfWork.Bookings
            .GetTableNoTracking()
            .Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
            return Errors.BookingNotFound;

        if (booking.TechnicianId != user.Id && booking.ServiceRequest.Customer.Id != user.Id)
            return Errors.Unauthorized;

        var query = unitOfWork.ChatMessages.GetTableNoTracking().Include(x => x.Conversation)
            .Where(x => x.Conversation.ServiceBookingId == request.BookingId)
            .OrderByDescending(x => x.SentAt);

        var paginated = await query.Select(x => x.ToChatMessageResponse())
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        return paginated;
    }
}
