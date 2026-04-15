using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Wallets.Queries.GetWalletBalance;

public sealed class GetWalletBalanceQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetWalletBalanceQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        var wallet = await unitOfWork.Wallets.GetTableNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == currentUser.Id);

        if (wallet == null)
            return Errors.WalletNotFound;

        return wallet.Balance;
    }
}
