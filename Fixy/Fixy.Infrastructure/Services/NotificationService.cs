using Fixy.Application.Abstracts;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Fixy.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IRealtimeNotificationSender _realtimeSender;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IRealtimeNotificationSender realtimeSender, IUnitOfWork unitOfWork)
    {
        _realtimeSender = realtimeSender;
        _unitOfWork = unitOfWork;
    }

    #region Generic Notification Methods

    public async Task SendNotificationAsync(Guid userId, string title, string message, NotificationType type, object data = null)
    {
        // 1. Save to database
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Data = data != null ? JsonSerializer.Serialize(data) : null,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);

        // 2. Send real-time notification
        await _realtimeSender.SendToUserAsync(
            userId,
            "ReceiveNotification",
            new
            {
                id = notification.Id,
                title = title,
                message = message,
                type = type.ToString(),
                data = data,
                timestamp = DateTime.UtcNow
            }
        );
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendNotificationToGroupAsync(string groupName, string title, string message, NotificationType type, object data = null)
    {
        await _realtimeSender.SendToGroupAsync(
            groupName,
            "ReceiveNotification",
            new
            {
                title = title,
                message = message,
                type = type.ToString(),
                data = data,
                timestamp = DateTime.UtcNow
            }
        );
    }

    public async Task SendNotificationToAllAsync(string title, string message, NotificationType type, object data = null)
    {
        await _realtimeSender.SendToAllAsync(
            "ReceiveNotification",
            new
            {
                title = title,
                message = message,
                type = type.ToString(),
                data = data,
                timestamp = DateTime.UtcNow
            }
        );
    }

    public async Task SendToCustomerAsync(Guid customerId, string title, string message, NotificationType type, object data = null)
    {
        await SendNotificationAsync(customerId, title, message, type, data);
    }

    public async Task SendToTechnicianAsync(Guid technicianId, string title, string message, NotificationType type, object data = null)
    {
        await SendNotificationAsync(technicianId, title, message, type, data);
    }

    public async Task SendToAdminAsync(string title, string message, NotificationType type, object data = null)
    {
        await SendNotificationToGroupAsync("Admins", title, message, type, data);
    }

    #endregion

    #region Account Notifications

    public async Task NotifyAccountApprovedAsync(Guid userId)
    {
        await SendNotificationAsync(
            userId,
            "Account Approved!",
            $"Your account has been approved. You can now start using Fixy!",
            NotificationType.AccountApproved
        );
    }

    public async Task NotifyAccountRejectedAsync(Guid userId, string userType, string reason)
    {
        await SendNotificationAsync(
            userId,
            "Account Application Update",
            $"Your {userType} account application was not approved. Reason: {reason}",
            NotificationType.AccountRejected,
            new { userType, reason }
        );
    }

    public async Task NotifyAccountBlockedAsync(Guid userId, string reason)
    {
        await SendNotificationAsync(
            userId,
            "Account Blocked",
            $"Your account has been blocked. Reason: {reason}. Please contact support.",
            NotificationType.AccountBlocked,
            new { reason }
        );
    }

    #endregion

    #region Service Request Notifications

    public async Task NotifyNewServiceRequestAsync(Guid technicianId, object serviceRequest)
    {
        await SendToTechnicianAsync(
            technicianId,
            "New Service Request! 🔔",
            "A new service request is available in your area.",
            NotificationType.NewServiceRequest,
            serviceRequest
        );
    }

    public async Task NotifyServiceRequestUpdatedAsync(Guid technicianId, object serviceRequest)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Service Request Updated",
            "A service request you viewed has been updated with new information.",
            NotificationType.ServiceRequestUpdated,
            serviceRequest
        );
    }

    public async Task NotifyServiceRequestCancelledAsync(Guid technicianId, int serviceRequestId)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Service Request Cancelled",
            "A service request has been cancelled by the customer.",
            NotificationType.ServiceRequestCancelled,
            new { serviceRequestId }
        );
    }

    #endregion

    #region Offer Notifications

    public async Task NotifyOfferReceivedAsync(Guid customerId, object offer)
    {
        await SendToCustomerAsync(
            customerId,
            "New Price Offer! 💰",
            "You received a new price offer for your service request.",
            NotificationType.NewOfferReceived,
            offer
        );
    }

    public async Task NotifyOfferAcceptedAsync(Guid technicianId, object booking)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Offer Accepted! ✅",
            "Great news! Your offer has been accepted. The booking is now confirmed.",
            NotificationType.OfferAccepted,
            booking
        );
    }

    public async Task NotifyOfferRejectedAsync(Guid technicianId, int offerId)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Offer Not Selected",
            "The customer has chosen a different offer for this service request.",
            NotificationType.OfferRejected,
            new { offerId }
        );
    }

    public async Task NotifyOfferExpiredAsync(Guid technicianId, int offerId)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Offer Expired",
            "Your offer has expired as the service request was updated.",
            NotificationType.OfferExpired,
            new { offerId }
        );
    }

    #endregion

    #region Booking Notifications

    public async Task NotifyBookingCreatedAsync(Guid userId, object booking)
    {
        await SendNotificationAsync(
            userId,
            "Booking Confirmed! 📅",
            "Your booking has been created successfully.",
            NotificationType.BookingCreated,
            booking
        );
    }

    public async Task NotifyBookingConfirmedAsync(Guid userId, object booking)
    {
        await SendNotificationAsync(
            userId,
            "Booking Confirmed",
            "Your booking has been confirmed by the technician.",
            NotificationType.BookingConfirmed,
            booking
        );
    }

    public async Task NotifyBookingCancelledAsync(Guid userId, object booking, string reason)
    {
        await SendNotificationAsync(
            userId,
            "Booking Cancelled",
            $"Your booking has been cancelled. Reason: {reason}",
            NotificationType.BookingCancelled,
            new { booking, reason }
        );
    }

    #endregion

    #region Payment Notifications

    public async Task NotifyPaymentReceivedAsync(Guid userId, decimal amount, string bookingId)
    {
        await SendNotificationAsync(
            userId,
            "Payment Received ✓",
            $"Your payment of {amount:F2} EGP has been received and is being processed.",
            NotificationType.PaymentReceived,
            new { amount, bookingId }
        );
    }

    public async Task NotifyEscrowLockedAsync(Guid technicianId, decimal amount, string bookingId)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Payment Secured 🔒",
            $"{amount:F2} EGP is being held in escrow for booking #{bookingId}. Complete the service to receive payment.",
            NotificationType.EscrowLocked,
            new { amount, bookingId }
        );
    }

    public async Task NotifyEscrowReleasedAsync(Guid technicianId, decimal amount, string bookingId)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Payment Released! 💵",
            $"{amount:F2} EGP has been released and is now available for withdrawal.",
            NotificationType.EscrowReleased,
            new { amount, bookingId }
        );
    }

    public async Task NotifyRefundProcessedAsync(Guid customerId, decimal amount, string bookingId)
    {
        await SendToCustomerAsync(
            customerId,
            "Refund Processed",
            $"Your refund of {amount:F2} EGP has been processed for booking #{bookingId}.",
            NotificationType.RefundProcessed,
            new { amount, bookingId }
        );
    }

    public async Task NotifyWithdrawalCompletedAsync(Guid technicianId, decimal amount)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Withdrawal Completed ✓",
            $"Your withdrawal of {amount:F2} EGP has been processed successfully.",
            NotificationType.WithdrawalCompleted,
            new { amount }
        );
    }

    #endregion

    #region Service Completion Notifications

    public async Task NotifyServiceMarkedCompleteAsync(Guid customerId, object booking)
    {
        await SendToCustomerAsync(
            customerId,
            "Service Completed 🎉",
            "The technician has marked the service as completed. Please review and confirm.",
            NotificationType.ServiceMarkedComplete,
            booking
        );
    }

    public async Task NotifyServiceConfirmedAsync(Guid technicianId, object booking)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Service Confirmed! ✅",
            "The customer has confirmed service completion. Payment is being processed.",
            NotificationType.ServiceConfirmed,
            booking
        );
    }

    public async Task NotifyServiceRejectedAsync(Guid technicianId, object booking, string reason)
    {
        await SendToTechnicianAsync(
            technicianId,
            "Service Completion Rejected",
            $"The customer has not confirmed completion. Reason: {reason}",
            NotificationType.ServiceRejected,
            new { booking, reason }
        );
    }

    #endregion

    #region Feedback Notifications

    public async Task NotifyFeedbackReceivedAsync(Guid technicianId, object feedback)
    {
        await SendToTechnicianAsync(
            technicianId,
            "New Feedback Received ⭐",
            "A customer has left feedback on your service.",
            NotificationType.FeedbackReceived,
            feedback
        );
    }

    public async Task NotifyRatingUpdatedAsync(Guid technicianId, double newRating, double oldRating)
    {
        var change = newRating - oldRating;
        var emoji = change > 0 ? "📈" : change < 0 ? "📉" : "➡️";

        await SendToTechnicianAsync(
            technicianId,
            $"Rating Updated {emoji}",
            $"Your rating has been updated from {oldRating:F2} to {newRating:F2}",
            NotificationType.RatingUpdated,
            new { newRating, oldRating, change }
        );
    }

    #endregion

    #region Dispute Notifications

    public async Task NotifyDisputeRaisedAsync(Guid userId, object dispute)
    {
        await SendNotificationAsync(
            userId,
            "Dispute Raised ⚠️",
            "A dispute has been raised on one of your bookings. An admin will review it.",
            NotificationType.DisputeRaised,
            dispute
        );
    }

    public async Task NotifyDisputeResolvedAsync(Guid userId, object dispute, string resolution)
    {
        await SendNotificationAsync(
            userId,
            "Dispute Resolved",
            $"The dispute has been resolved. Resolution: {resolution}",
            NotificationType.DisputeResolved,
            new { dispute, resolution }
        );
    }

    #endregion

    #region Notification Management

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await _unitOfWork.Notifications.GetTableNoTracking().ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId);

        // Send updated unread count
        var unreadCount = await GetUnreadCountAsync(userId);
        await _realtimeSender.SendToUserAsync(
            userId,
            "UnreadCountUpdated",
            new { unreadCount }
        );
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);

        // Send updated unread count (0)
        await _realtimeSender.SendToUserAsync(
            userId,
            "UnreadCountUpdated",
            new { unreadCount = 0 }
        );
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        await _unitOfWork.Notifications.DeleteAsync(notificationId, userId);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion
}