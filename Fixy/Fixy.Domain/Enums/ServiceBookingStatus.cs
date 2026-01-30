namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    Active,
    PriceChangePendingCustomerApproval,
    CompletedPendingCustomerConfirmation,
    Completed,
    Cancelled
}
