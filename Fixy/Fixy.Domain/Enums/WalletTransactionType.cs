namespace Fixy.Domain.Enums;

public enum WalletTransactionType
{
    TopUp = 1,              // Technician/Customer loads money via Stripe
    BookingPayment = 2,     // Customer pays for service (debit)
    EarningsCredit = 3,     // Technician receives 85% from card job (credit)
    CommissionDebit = 4,    // 15% deducted from technician on cash job (debit)
    Refund = 5,             // Cancelled booking returned to customer (credit)
    Withdrawal = 6,         // Technician cashes out to bank (debit)
}
