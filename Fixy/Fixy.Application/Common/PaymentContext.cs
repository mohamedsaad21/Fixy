using Fixy.Application.Contracts.Services;

namespace Fixy.Application.Common;

public class PaymentContext
{
    private IPaymentService _paymentService;

    public PaymentContext(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public void SwitchPaymentService(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
}
