namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    Active,
    PriceChangePendingCustomerApproval,
    CompletedPendingCustomerConfirmation,
    PaymentPending,
    PaymentRecieved,
    PaymentFailed,
    Completed,
    Cancelled,
    Disputed
}