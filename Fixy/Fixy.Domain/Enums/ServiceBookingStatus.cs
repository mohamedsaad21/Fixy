namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress = 1,
    AwaitingPriceChangeApproval,
    AwaitingCustomerConfirmationForCompletion,
    AwaitingPayment,
    CancelledByTechnician,
    CancelledByCustomer,
    Completed,
    Disputed
}