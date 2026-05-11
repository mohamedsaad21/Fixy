using Fixy.Application.Common.DTOs.Chat;
using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    public ChatHub(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task SendMessage(Guid bookingId, MessageContent messageContent)
    {
        // Get sender
        var senderIdStr = Context.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(senderIdStr))
            return;

        var senderId = Guid.Parse(senderIdStr);

        // Get booking
        var booking = await _unitOfWork.Bookings
            .GetTableNoTracking().Include(x => x.ServiceRequest)
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking == null)
            return;

        // Validate sender belongs to booking
        if (booking.ServiceRequest.CustomerId != senderId && booking.TechnicianId != senderId)
            return;

        // Determine receiver
        var receiverId = booking.ServiceRequest.CustomerId == senderId
            ? booking.TechnicianId
            : booking.ServiceRequest.CustomerId;

        // Get or create conversation (per booking)
        var conversation = await _unitOfWork.Conversations
            .GetOrCreateAsync(booking.Id, booking.ServiceRequest.CustomerId, booking.TechnicianId);

        if (conversation.IsClosed)
            return;

        var sender = await _userManager.FindByIdAsync(senderId.ToString());
        // Save message
        var msg = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = messageContent.Content,
            Attachment = messageContent.Attachment,
            SentAt = DateTimeOffset.UtcNow,
            IsSeen = false
        };

        await _unitOfWork.ChatMessages.AddAsync(msg);
        await _unitOfWork.SaveChangesAsync();

        // Send to receiver (group per user)
        await Clients.Group($"user_{receiverId}")
            .SendAsync("ReceiveMessage", new
            {
                msg.Id,
                msg.ConversationId,
                msg.SenderId,
                SenderName = sender.FirstName + " " + sender.LastName,
                SenderUserName = sender.UserName,
                SenderProfilePicture = sender.ProfilePictureUrl,
                msg.ReceiverId,
                msg.Content,
                msg.Attachment,
                msg.SentAt,
                msg.IsSeen
            });

        // Echo back to sender
        await Clients.Caller.SendAsync("MessageSent", new
        {
            msg.Id,
            msg.ConversationId,
            msg.SenderId,
            msg.ReceiverId,
            msg.Content,
            msg.Attachment,
            msg.SentAt
        });
    }


    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("uid")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("uid")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
