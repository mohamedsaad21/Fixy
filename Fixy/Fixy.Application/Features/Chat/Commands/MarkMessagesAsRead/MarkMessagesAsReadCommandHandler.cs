using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Chat.Commands.MarkMessagesAsRead;

public sealed class MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
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

        var messages = await unitOfWork.ChatMessages
            .GetTableAsTracking().Include(x => x.Conversation)
            .Where(x => x.Conversation.ServiceBookingId == request.BookingId && x.SenderId != user.Id && !x.IsSeen)
            .ToListAsync();

        foreach (var msg in messages)
        {
            msg.IsSeen = true;
            msg.SeenAt = DateTime.UtcNow;
        }

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
