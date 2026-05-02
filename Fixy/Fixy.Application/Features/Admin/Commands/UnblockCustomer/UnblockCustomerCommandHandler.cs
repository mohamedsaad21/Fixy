using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.Admin.Commands.UnblockCustomer;

public sealed class UnblockCustomerCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<UnblockCustomerCommand, Result>
{
    public async Task<Result> Handle(UnblockCustomerCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var customer = await unitOfWork.Customers.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId);

        if (customer == null)
            return Errors.CustomerNotFound;

        if (customer.Status == CustomerStatus.Active)
            return Errors.CustomerAlreadyActive;

        customer.Status = CustomerStatus.Active;
        customer.BlockReason = null;
        customer.BlockedAt = null;
        customer.BlockedBy = null;

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
