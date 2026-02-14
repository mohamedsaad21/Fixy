using Fixy.Application.Abstracts;
using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Technicians.Queries.GetStripeStatus;

public class GetStripeStatusQueryHandler : IRequestHandler<GetStripeStatusQuery, Result<GetStripeStatusResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStripeConnectService _stripeConnectService;

    public GetStripeStatusQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IStripeConnectService stripeConnectService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _stripeConnectService = stripeConnectService;
    }

    public async Task<Result<GetStripeStatusResponse>> Handle(GetStripeStatusQuery request, CancellationToken cancellationToken)
    {
        var technicianId = _currentUserService.GetCurrentUserId();
        var account = await _unitOfWork.TechnicianStripeAccounts.GetTableNoTracking()
            .FirstOrDefaultAsync(a => a.TechnicianId == technicianId);

        if (account == null)
            return new GetStripeStatusResponse { IsOnboarded = false, CanReceivePayments = false, Status = "Not yet onboarded" };

        var isReady = await _stripeConnectService.IsAccountFullyOnboardedAsync(account.StripeAccountId);

        return new GetStripeStatusResponse
        {
            IsOnboarded = isReady,
            CanReceivePayments = account.PayoutsEnabled,
            Status = account.OnboardingStatus
        };
    }
}
