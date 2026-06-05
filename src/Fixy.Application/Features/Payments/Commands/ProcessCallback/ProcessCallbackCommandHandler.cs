using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ProcessCallbackCommandHandler(IPaymentService paymentService, ILogger<ProcessCallbackCommandHandler> logger) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var isValid = await paymentService.VerifyWebhookSignature(request.Payload, request.Signature);

            if (!isValid)
            {
                logger.LogWarning("Invalid webhook signature");
                return Errors.InvalidHmacSignature;
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing webhook");
            return Errors.CallbackProcessingFailed;
        }
    }
}