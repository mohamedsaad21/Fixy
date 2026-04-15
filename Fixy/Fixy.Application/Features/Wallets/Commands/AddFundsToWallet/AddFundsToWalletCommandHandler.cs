using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Features.Wallets.Commands.AddFundsToWallet.Responses;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Wallets.Commands.AddFundsToWallet;

public sealed class AddFundsToWalletCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IPaymentService paymentService) : IRequestHandler<AddFundsToWalletCommand, Result<AddFundsToWalletResponse>>
{
    public async Task<Result<AddFundsToWalletResponse>> Handle(AddFundsToWalletCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        var wallet = await unitOfWork.Wallets.GetTableNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == currentUser.Id);

        if (wallet == null)
            return Errors.WalletNotFound;

        var paymentUrlResult = await paymentService.CreateTopUpSessionAsync(
            amount: request.Amount,
            userId: currentUser.Id.ToString(),
            customerName: $"{currentUser.FullName}",
            customerEmail: currentUser.Email!,
            customerPhone: currentUser.PhoneNumber!);

        var transaction = new WalletTransaction
        {
            WalletId = wallet.Id,
            Amount = request.Amount,
            BalanceBefore = wallet.Balance,
            BalanceAfter = wallet.Balance + request.Amount,
            Type = WalletTransactionType.TopUp,
            ReferenceId = paymentUrlResult.MerchantOrderId,
            StripeSessionId = paymentUrlResult.StripeSessionId,
            Description = $"Wallet top-up of {request.Amount} EGP",
        };

        await unitOfWork.WalletTransactions.AddAsync(transaction);
        await unitOfWork.SaveChangesAsync();

        return new AddFundsToWalletResponse
        {
            PaymentUrl = paymentUrlResult.PaymentUrl,
            StripeSessionId = paymentUrlResult.StripeSessionId,
            MerchantOrderId = paymentUrlResult.MerchantOrderId,
            Amount = request.Amount
        };
    }
}
