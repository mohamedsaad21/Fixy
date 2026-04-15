using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public sealed class ProcessCallbackCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ProcessCallbackCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessCallbackCommand request, CancellationToken cancellationToken)
    {
        // Delegate ALL Stripe logic to the service
        var callbackResult = await paymentService.ProcessCallbackAsync();

        // Handle unrecognized/unsubscribed events gracefully
        if (callbackResult == null)
            return true; // Stripe expects 200 OK for unhandled events

        // Find pending transaction by MerchantOrderId
        var transaction = await unitOfWork.WalletTransactions.GetTableAsTracking().FirstOrDefaultAsync(x => x.ReferenceId == callbackResult.MerchantOrderId);

        if (transaction == null)
            return Errors.TransactionNotFound;

        // Idempotency check — Stripe can fire webhook more than once
        if (transaction.Status == WalletTransactionStatus.Success)
            return true;

        // Get wallet
        var wallet = await unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == transaction.WalletId);

        if (wallet == null)
            return Errors.WalletNotFound;

        // pply balance — single SaveChangesAsync for atomicity
        transaction.Status = WalletTransactionStatus.Success;
        transaction.UpdatedAt = DateTime.UtcNow;

        wallet.Balance += transaction.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        return true;
    }
}