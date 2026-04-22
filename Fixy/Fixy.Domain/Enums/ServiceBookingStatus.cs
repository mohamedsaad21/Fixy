namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress,
    PriceChangePendingCustomerApproval,
    Completed,
    AwaitingPayment,
    Paid,
    Cancelled,
    Disputed
}