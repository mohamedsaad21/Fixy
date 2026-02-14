using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Payments.Commands.HandleStripePaymentSucceeded;

public class HandleStripePaymentSucceededCommandHandler : IRequestHandler<HandleStripePaymentSucceededCommand, Result>
{
    private readonly IStripeWebhookService _stripeWebhookService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HandleStripePaymentSucceededCommandHandler(IStripeWebhookService stripeWebhookService, IHttpContextAccessor httpContextAccessor)
    {
        _stripeWebhookService = stripeWebhookService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(HandleStripePaymentSucceededCommand request, CancellationToken cancellationToken)
    {
        var json = await new StreamReader(_httpContextAccessor.HttpContext.Request.Body).ReadToEndAsync();

        // Get Stripe signature from headers
        var signature = _httpContextAccessor.HttpContext.Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrEmpty(signature))
            return Errors.MissingStripeSignature;

        // Process webhook
        var success = await _stripeWebhookService.HandleWebhookAsync(json, signature);

        if (!success)
            return Errors.WebhookProcessingFailed;

        return Result.Success();
    }
}
