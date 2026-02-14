using Fixy.Domain.Entities;
using Fixy.Domain.Enums;

namespace Fixy.Application.Abstracts;

public interface INotificationService
{
    // Generic notification methods
    Task SendNotificationAsync(Guid userId, string title, string message, NotificationType type, object data = null);
    Task SendNotificationToGroupAsync(string groupName, string title, string message, NotificationType type, object data = null);
    Task SendNotificationToAllAsync(string title, string message, NotificationType type, object data = null);

    // User-specific methods
    Task SendToCustomerAsync(Guid customerId, string title, string message, NotificationType type, object data = null);
    Task SendToTechnicianAsync(Guid technicianId, string title, string message, NotificationType type, object data = null);
    Task SendToAdminAsync(string title, string message, NotificationType type, object data = null);

    // Account notifications
    Task NotifyAccountApprovedAsync(Guid userId);
    Task NotifyAccountRejectedAsync(Guid userId, string userType, string reason);
    Task NotifyAccountBlockedAsync(Guid userId, string reason);

    // Service Request notifications
    Task NotifyNewServiceRequestAsync(Guid technicianId, object serviceRequest);
    Task NotifyServiceRequestUpdatedAsync(Guid technicianId, object serviceRequest);
    Task NotifyServiceRequestCancelledAsync(Guid technicianId, int serviceRequestId);

    // Offer notifications
    Task NotifyOfferReceivedAsync(Guid customerId, object offer);
    Task NotifyOfferAcceptedAsync(Guid technicianId, object booking);
    Task NotifyOfferRejectedAsync(Guid technicianId, int offerId);
    Task NotifyOfferExpiredAsync(Guid technicianId, int offerId);

    // Booking notifications
    Task NotifyBookingCreatedAsync(Guid userId, object booking);
    Task NotifyBookingConfirmedAsync(Guid userId, object booking);
    Task NotifyBookingCancelledAsync(Guid userId, object booking, string reason);

    // Payment notifications
    Task NotifyPaymentReceivedAsync(Guid userId, decimal amount, string bookingId);
    Task NotifyEscrowLockedAsync(Guid technicianId, decimal amount, string bookingId);
    Task NotifyEscrowReleasedAsync(Guid technicianId, decimal amount, string bookingId);
    Task NotifyRefundProcessedAsync(Guid customerId, decimal amount, string bookingId);
    Task NotifyWithdrawalCompletedAsync(Guid technicianId, decimal amount);

    // Service Completion notifications
    Task NotifyServiceMarkedCompleteAsync(Guid customerId, object booking);
    Task NotifyServiceConfirmedAsync(Guid technicianId, object booking);
    Task NotifyServiceRejectedAsync(Guid technicianId, object booking, string reason);

    // Feedback notifications
    Task NotifyFeedbackReceivedAsync(Guid technicianId, object feedback);
    Task NotifyRatingUpdatedAsync(Guid technicianId, double newRating, double oldRating);

    // Dispute notifications
    Task NotifyDisputeRaisedAsync(Guid userId, object dispute);
    Task NotifyDisputeResolvedAsync(Guid userId, object dispute, string resolution);

    // Notification management
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
}