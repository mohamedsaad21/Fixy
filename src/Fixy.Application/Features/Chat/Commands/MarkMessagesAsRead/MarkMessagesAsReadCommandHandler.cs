using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Chat.Commands.MarkMessagesAsRead;

public sealed class MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<MarkMessagesAsReadCommandHandler>logger) : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking messages as read. BookingId: {BookingId}", request.BookingId);

        var user = await currentUserService.GetCurrentUserAsync();
        if(user == null)
        {
            logger.LogWarning("Mark messages as read failed — no current user resolved. BookingId: {BookingId}", request.BookingId);
            return Errors.Unauthorized;
        }

        var booking = await unitOfWork.Bookings
            .GetTableNoTracking()
            .Include(x => x.ServiceRequest)
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.BookingId);

        if (booking == null)
        {
            logger.LogWarning("Mark messages as read failed — booking not found. BookingId: {BookingId}, UserId: {UserId}", request.BookingId, user.Id);
            return Errors.BookingNotFound;
        }

        if (booking.TechnicianId != user.Id && booking.ServiceRequest.Customer.Id != user.Id)
        {
            logger.LogWarning("Mark messages as read failed — user is not a participant in this booking. BookingId: {BookingId}, UserId: {UserId}, TechnicianId: {TechnicianId}, CustomerId: {CustomerId}", request.BookingId, user.Id, booking.TechnicianId, booking.ServiceRequest.Customer.Id);
            return Errors.Unauthorized;
        }

        var messages = await unitOfWork.ChatMessages
            .GetTableAsTracking().Include(x => x.Conversation)
            .Where(x => x.Conversation.ServiceBookingId == request.BookingId && x.SenderId != user.Id && !x.IsSeen)
            .ToListAsync();

        var unreadCount = messages.Count;

        if (unreadCount == 0)
        {
            logger.LogInformation("No unread messages to mark. BookingId: {BookingId}, UserId: {UserId}", request.BookingId, user.Id);
            return Result.Success();
        }

        foreach (var msg in messages)
        {
            msg.IsSeen = true;
            msg.SeenAt = DateTime.UtcNow;
        }

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Messages marked as read successfully. BookingId: {BookingId}, UserId: {UserId}, MarkedCount: {MarkedCount}", request.BookingId, user.Id, unreadCount);
        return Result.Success();
    }
}
