using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.UnblockCustomer;

public sealed class UnblockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UnblockCustomerCommandHandler> logger) : IRequestHandler<UnblockCustomerCommand, Result>
{
    public async Task<Result> Handle(UnblockCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to unblock customer. CustomerId: {CustomerId}", request.CustomerId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Unblock customer failed — unauthorized, no current user resolved. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.Unauthorized;
        }

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
        {
            logger.LogWarning("Unblock customer failed — customer not found. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerNotFound;
        }

        if (customer.Status == CustomerStatus.Active)
        {
            logger.LogWarning("Unblock customer skipped — customer is already active. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerAlreadyActive;
        }

        customer.Status = CustomerStatus.Active;
        customer.BlockReason = null;
        customer.BlockedAt = null;
        customer.BlockedBy = null;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Customer successfully unblocked. CustomerId: {CustomerId}, AdminId: {AdminId}", request.CustomerId, currentUser.Id);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            customer.Id,
            NotificationType.CustomerUnblocked,
            SharedResourcesKeys.NotificationCustomerUnblockedTitle,
            SharedResourcesKeys.NotificationCustomerUnblockedBody
        ));

        return Result.Success();
    }
}
