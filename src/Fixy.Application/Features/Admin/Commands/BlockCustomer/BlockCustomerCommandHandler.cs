using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.Admin.Commands.BlockCustomer;

public sealed class BlockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<BlockCustomerCommandHandler> logger) : IRequestHandler<BlockCustomerCommand, Result>
{
    public async Task<Result> Handle(BlockCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Admin attempting to block customer. CustomerId: {CustomerId}", request.CustomerId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            logger.LogWarning("Block customer failed — unauthorized, no current user resolved. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.Unauthorized;
        }

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
        {
            logger.LogWarning("Block customer failed — customer not found. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerNotFound;
        }

        if (customer.Status == CustomerStatus.Blocked)
        {
            logger.LogWarning("Block customer skipped — already blocked. CustomerId: {CustomerId}, BlockedBy: {BlockedBy}", request.CustomerId, customer.BlockedBy);
            return Errors.CustomerAlreadyBlocked;
        }

        customer.Status = CustomerStatus.Blocked;
        customer.BlockReason = request.Reason;
        customer.BlockedAt = DateTime.UtcNow;
        customer.BlockedBy = currentUser.Id;

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Customer successfully blocked. CustomerId: {CustomerId}, AdminId: {AdminId}, Reason: {Reason}", request.CustomerId, currentUser.Id, request.Reason);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            customer.Id,
            NotificationType.CustomerBlocked,
            SharedResourcesKeys.NotificationCustomerBlockedTitle,
            SharedResourcesKeys.NotificationCustomerBlockedBody
        ));

        return Result.Success();
    }
}
