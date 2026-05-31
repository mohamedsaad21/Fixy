namespace Fixy.Domain.Enums;

public enum ServiceBookingStatus
{
    InProgress = 1,
    AwaitingPriceChangeApproval,
    AwaitingCustomerConfirmationForCompletion,
    AwaitingPayment,
    AwaitingFeedback,
    CancelledByTechnician,
    CancelledByCustomer,
    CustomerCompleted,
    TechnicianCompleted,
    FullCompleted,
    Disputed
}