using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.PayBooking;

public class PayBookingCommandHandler : IRequestHandler<PayBookingCommand, Result>
{
    private readonly IPaymentService _paymentService;

    public PayBookingCommandHandler(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(PayBookingCommand request, CancellationToken cancellationToken)
    {
        await _paymentService.CreateOrUpdatePaymentIntentAsync(request.BookingId);
        return Result.Success();
    }
}
