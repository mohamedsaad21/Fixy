using Fixy.Api.Base;
using Fixy.Api.Contracts.Routing;
using Fixy.Application.Features.Wallets.Commands.AddFundsToWallet;
using Fixy.Application.Features.Wallets.Queries.GetWalletBalance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fixy.Api.Controllers;

public class WalletController : AppControllerBase
{
    [Authorize]
    [HttpGet(Router.WalletRouting.GetWalletBalance)]
    public async Task<IActionResult> GetWalletBalance()
    {
        return ToActionResult(await Mediator.Send(new GetWalletBalanceQuery()));
    }

    [Authorize]
    [HttpPost(Router.WalletRouting.AddFundsToWallet)]
    public async Task<IActionResult> AddFundsToWallet([FromForm] decimal Amount)
    {
        return ToActionResult(await Mediator.Send(new AddFundsToWalletCommand(Amount)));
    }
}
