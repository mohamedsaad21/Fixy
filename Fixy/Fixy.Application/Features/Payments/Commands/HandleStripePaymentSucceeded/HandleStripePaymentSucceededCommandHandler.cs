using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Application.Features.Payments.Commands.HandleStripePaymentSucceeded;

public class HandleStripePaymentSucceededCommandHandler : IRequestHandler<HandleStripePaymentSucceededCommand, Result>
{
    private readonly IPaymentService _paymentService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HandleStripePaymentSucceededCommandHandler(IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
    {
        _paymentService = paymentService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(HandleStripePaymentSucceededCommand request, CancellationToken cancellationToken)
    {
        var json = await new StreamReader(_httpContextAccessor.HttpContext.Response.Body).ReadToEndAsync();
        await _paymentService.UpdateBookingPaymentStatusAsync(json, _httpContextAccessor.HttpContext.Request.Headers["Stripe-Signature"]!);
        return Result.Success();
    }
}
