namespace Fixy.Domain.Enums;

public enum NotificationType
{
    PriceOfferAccepted = 1,
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
    TechnicianBlocked
}
