namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress,
    AwaitingPriceChangeApproval,
    AwaitingCustomerConfirmationForCompletion,
    AwaitingPayment,
    Completed,
    Cancelled,
    Disputed
}