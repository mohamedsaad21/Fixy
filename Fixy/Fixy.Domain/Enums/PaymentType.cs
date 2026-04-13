namespace Fixy.Domain.Enums;

public enum PaymentType
{
    CustomerPayment,    // Customer pays for service
    TechnicianPayout,   // Platform pays technician
    Refund,            // Refund to customer
    Commission         // Commission payment
}
