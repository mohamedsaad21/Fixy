using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.PayBooking;

public class PayBookingCommandHandler : IRequestHandler<PayBookingCommand, Result>
{
    private readonly IStripePaymentService _paymentService;

    public PayBookingCommandHandler(IStripePaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(PayBookingCommand request, CancellationToken cancellationToken)
    {
        await _paymentService.CreatePaymentIntentAsync(request.BookingId);
        return Result.Success();
    }
}
