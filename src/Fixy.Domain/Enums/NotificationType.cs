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
    BookingCompleted,
    TechnicianApproved,
    TechnicianRejected,
    TechnicianBlocked,
    TechnicianUnblocked,
    CustomerBlocked,
    CustomerUnblocked
}
