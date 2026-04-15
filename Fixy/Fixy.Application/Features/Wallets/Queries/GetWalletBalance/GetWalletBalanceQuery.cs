using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Wallets.Queries.GetWalletBalance;

public sealed record GetWalletBalanceQuery() : IRequest<Result<decimal>>;