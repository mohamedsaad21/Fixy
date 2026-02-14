namespace Fixy.Domain.Enums;

public enum NotificationType
{
    // Account related
    AccountApproved = 1,
    AccountRejected = 2,
    AccountBlocked = 3,

    // Service Request related
    NewServiceRequest = 10,
    ServiceRequestUpdated = 11,
    ServiceRequestCancelled = 12,

    // Offer related
    NewOfferReceived = 20,
    OfferAccepted = 21,
    OfferRejected = 22,
    OfferExpired = 23,

    // Booking related
    BookingCreated = 30,
    BookingConfirmed = 31,
    BookingCancelled = 32,

    // Payment related
    PaymentReceived = 40,
    PaymentProcessed = 41,
    EscrowLocked = 42,
    EscrowReleased = 43,
    RefundProcessed = 44,
    WithdrawalCompleted = 45,

    // Service Completion related
    ServiceMarkedComplete = 50,
    ServiceConfirmed = 51,
    ServiceRejected = 52,

    // Feedback related
    FeedbackReceived = 60,
    RatingUpdated = 61,

    // Dispute related
    DisputeRaised = 70,
    DisputeResolved = 71,
    DisputeEscalated = 72,

    // Admin related
    MisconductReported = 80,
    SystemAlert = 90,

    // General
    GeneralNotification = 100
}