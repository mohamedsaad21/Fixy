using Fixy.Application.Bases;
using Fixy.Application.Features.Wallets.Commands.AddFundsToWallet.Responses;
using MediatR;

namespace Fixy.Application.Features.Wallets.Commands.AddFundsToWallet;

public sealed record AddFundsToWalletCommand
    (
        decimal Amount
    ) : IRequest<Result<AddFundsToWalletResponse>>;