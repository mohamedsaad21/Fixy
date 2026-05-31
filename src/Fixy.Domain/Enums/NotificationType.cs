namespace Fixy.Domain.Enums;

public enum NotificationType
{
    PriceOfferReceived = 1,
    PriceOfferAccepted,
    BookingCancelledByCustomer,
    BookingCancelledByTechnician,
    PriceChangeRequested,
    PriceChangeApproved,
    PriceChangeRejected,
    TechnicianCompleted,
    BookingAwaitingPayment,
    BookingPaymentSucceeded,
    BookingPaymentReceived,
    BookingCompleted,
    TechnicianApproved,
    TechnicianRejected,
    TechnicianBlocked,
    TechnicianUnblocked,
    CustomerBlocked,
    CustomerUnblocked
}
