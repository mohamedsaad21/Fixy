using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fixy.Application.Features.Admin.Commands.UnblockCustomer;

public sealed class UnblockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<UnblockCustomerCommand, Result>
{
    public async Task<Result> Handle(UnblockCustomerCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Admin attempting to unblock customer. CustomerId: {CustomerId}", request.CustomerId);

        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
        {
            Log.Warning("Unblock customer failed — unauthorized, no current user resolved. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.Unauthorized;
        }

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
        {
            Log.Warning("Unblock customer failed — customer not found. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerNotFound;
        }

        if (customer.Status == CustomerStatus.Active)
        {
            Log.Warning("Unblock customer skipped — customer is already active. CustomerId: {CustomerId}", request.CustomerId);
            return Errors.CustomerAlreadyActive;
        }

        customer.Status = CustomerStatus.Active;
        customer.BlockReason = null;
        customer.BlockedAt = null;
        customer.BlockedBy = null;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.CustomerUnblocked,
            SharedResourcesKeys.NotificationCustomerUnblockedTitle,
            SharedResourcesKeys.NotificationCustomerUnblockedBody
        );

        await unitOfWork.SaveChangesAsync();
        Log.Information("Customer successfully unblocked. CustomerId: {CustomerId}, AdminId: {AdminId}", request.CustomerId, currentUser.Id);
        return Result.Success();
    }
}
