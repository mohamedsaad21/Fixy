using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.BlockCustomer;

public sealed class BlockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<BlockCustomerCommand, Result>
{
    public async Task<Result> Handle(BlockCustomerCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
            return Errors.CustomerNotFound;

        if (customer.Status == CustomerStatus.Blocked)
            return Errors.CustomerAlreadyBlocked;

        customer.Status = CustomerStatus.Blocked;
        customer.BlockReason = request.Reason;
        customer.BlockedAt = DateTime.UtcNow;
        customer.BlockedBy = currentUser.Id;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.TechnicianApproved,
            SharedResourcesKeys.NotificationCustomerBlockedTitle,
            SharedResourcesKeys.NotificationCustomerBlockedBody
        );

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
