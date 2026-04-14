namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    Pending,
    InProgress,
    PriceChangePendingCustomerApproval,
    Completed,
    AwaitingPayment,
    Paid,
    Cancelled,
    Disputed
}