using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Admin.Commands.BlockCustomer;

public sealed class BlockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<BlockCustomerCommand, Result>
{
    public async Task<Result> Handle(BlockCustomerCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Admin attempting to block customer. CustomerId: {CustomerId}", request.CustomerId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            Log.Warning("Block customer failed — unauthorized, no current user resolved. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.Unauthorized;
        }

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
        {
            Log.Warning("Block customer failed — customer not found. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerNotFound;
        }

        if (customer.Status == CustomerStatus.Blocked)
        {
            Log.Warning("Block customer skipped — already blocked. CustomerId: {CustomerId}, BlockedBy: {BlockedBy}", request.CustomerId, customer.BlockedBy);
            return Errors.CustomerAlreadyBlocked;
        }

        customer.Status = CustomerStatus.Blocked;
        customer.BlockReason = request.Reason;
        customer.BlockedAt = DateTime.UtcNow;
        customer.BlockedBy = currentUser.Id;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.CustomerBlocked,
            SharedResourcesKeys.NotificationCustomerBlockedTitle,
            SharedResourcesKeys.NotificationCustomerBlockedBody
        );

        await unitOfWork.SaveChangesAsync();
        Log.Information("Customer successfully blocked. CustomerId: {CustomerId}, AdminId: {AdminId}, Reason: {Reason}", request.CustomerId, currentUser.Id, request.Reason);
        return Result.Success();
    }
}
