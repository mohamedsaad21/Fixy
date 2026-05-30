using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Payments.Commands.ProcessCallback;
using MediatR;
using Serilog;

public sealed class ProcessCallbackCommandHandler(IPaymentService paymentService) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var isValid = await paymentService.VerifyWebhookSignature(request.Payload, request.Signature);

            if (!isValid)
            {
                Log.Warning("Invalid webhook signature");
                return Errors.InvalidHmacSignature;
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing webhook");
            return Errors.CallbackProcessingFailed;
        }
    }
}