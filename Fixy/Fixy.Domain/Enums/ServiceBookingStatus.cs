namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress,
    PriceChangePendingCustomerApproval,
    TechnicianCompleted,
    AwaitingPayment,
    Completed,
    Cancelled,
    Disputed
}