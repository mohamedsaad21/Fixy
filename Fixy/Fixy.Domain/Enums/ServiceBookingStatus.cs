namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress,
    PriceChangePendingCustomerApproval,
    AwaitingPayment,
    Completed,
    Cancelled,
    Disputed
}