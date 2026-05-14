using Fixy.Application.Common.DTOs.Chat;
using Fixy.Application.Contracts.Services;
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
    private readonly IPresenceService _presenceService;
    private readonly IHubContext<NotificationHub> _notificationHub;
    public ChatHub(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, 
        IPresenceService presenceService, IHubContext<NotificationHub> notificationHub)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _presenceService = presenceService;
        _notificationHub = notificationHub;
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

        var isReceiverOnline = await _presenceService.IsOnlineAsync(receiverId);
        var isReceiverInConvo = await _presenceService.IsInConversationAsync(
            receiverId, conversation.Id
        );

        if (isReceiverInConvo)
        {
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
        }
        else if (isReceiverOnline)
        {
            // User is in the app but on a different page — send notification only
            await _notificationHub.Clients.Group($"user_{receiverId}")
            .SendAsync("ReceiveNotification", new {
                msg.ConversationId,
                msg.SenderId,
                SenderName = sender.FirstName + " " + sender.LastName,
                SenderProfilePicture = sender.ProfilePictureUrl,
                Preview = msg.Content?[..Math.Min(60, msg.Content.Length)],
                msg.SentAt
            });
        }
        else
        {
            // User is fully offline — send push notification (FCM / APNs)
            //await _pushNotificationService.SendAsync(receiverId, new PushPayload
            //{
            //    Title = sender.FirstName + " " + sender.LastName,
            //    Body = msg.Content?[..Math.Min(60, msg.Content.Length)],
            //    Data = new { conversationId = msg.ConversationId }
            //});
        }



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

            var userGuid = Guid.Parse(userId);

            // Clean up any active conversation tracking
            await _presenceService.LeaveConversationAsync(userGuid);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var userIdStr = Context.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return;

        var userId = Guid.Parse(userIdStr);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
        await _presenceService.JoinConversationAsync(userId, conversationId);

        await MarkMessagesSeen(conversationId);
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        var userIdStr = Context.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return;

        var userId = Guid.Parse(userIdStr);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{conversationId}");
        await _presenceService.LeaveConversationAsync(userId);
    }

    public async Task SendTypingIndicator(Guid conversationId, bool isTyping)
    {
        var userIdStr = Context.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return;

        var senderId = Guid.Parse(userIdStr);

        // Get the conversation to find the other user
        var conversation = await _unitOfWork.Conversations
            .GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == conversationId);

        if (conversation == null) return;

        // Determine the other user
        var otherUserId = conversation.CustomerId == senderId
            ? conversation.TechnicianId
            : conversation.CustomerId;

        // Only relay if the other user is in the conversation screen
        var isOtherInConvo = await _presenceService.IsInConversationAsync(otherUserId, conversationId);
        if (!isOtherInConvo) return;

        await Clients.Group($"user_{otherUserId}")
            .SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                UserId = senderId,
                IsTyping = isTyping
            });
    }

    public async Task MarkMessagesSeen(Guid conversationId)
    {
        var userIdStr = Context.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return;

        var userId = Guid.Parse(userIdStr);

        // Get all unseen messages sent TO this user in this conversation
        var unseenMessages = await _unitOfWork.ChatMessages
            .GetTableAsTracking()
            .Where(m => m.ConversationId == conversationId
                     && m.ReceiverId == userId
                     && !m.IsSeen)
            .ToListAsync();

        if (!unseenMessages.Any()) return;

        // Mark all as seen
        foreach (var m in unseenMessages)
            m.IsSeen = true;

        await _unitOfWork.SaveChangesAsync();

        // Tell the sender their messages were read
        var senderId = unseenMessages.First().SenderId;

        await Clients.Group($"user_{senderId}")
            .SendAsync("MessagesSeen", new
            {
                ConversationId = conversationId,
                SeenBy = userId,
                SeenAt = DateTimeOffset.UtcNow
            });
    }
}
