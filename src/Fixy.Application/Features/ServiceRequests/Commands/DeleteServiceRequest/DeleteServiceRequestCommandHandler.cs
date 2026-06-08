using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public sealed class DeleteServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteServiceRequestCommandHandler> logger) : IRequestHandler<DeleteServiceRequestCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceRequestCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to cancel service request. ServiceRequestId: {ServiceRequestId}", request.Id);
        var user = await currentUserService.GetCurrentUserAsync();
        
        if (user == null)
        {
            logger.LogWarning("Service request cancellation failed — no current user resolved. ServiceRequestId: {ServiceRequestId}",
                request.Id);
            return Errors.Unauthorized;
        }

        var serviceRequest = await unitOfWork.ServiceRequests
            .GetTableAsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
        {
            logger.LogWarning("Service request cancellation failed — service request not found. ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}", request.Id, user.Id);
            return Errors.ServiceRequestNotFound;
        }

        if (serviceRequest.CustomerId != user.Id)
        {
            logger.LogWarning("Service request cancellation failed — unauthorized customer. ServiceRequestId: {ServiceRequestId}, ServiceRequestCustomerId: {ServiceRequestCustomerId}, RequestingUserId: {RequestingUserId}", request.Id, serviceRequest.CustomerId, user.Id);
            return Errors.Unauthorized;
        }

        if (serviceRequest.Status == ServiceRequestStatus.Assigned)
        {
            logger.LogWarning("Service request cancellation failed — service request already accepted. ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}, CurrentStatus: {CurrentStatus}", request.Id, user.Id, serviceRequest.Status);
            return Errors.ServiceAlreadyAccepted;
        }

        serviceRequest.Status = ServiceRequestStatus.Cancelled;
        serviceRequest.IsDeleted = true;
        serviceRequest.DeletedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Service request cancelled successfully. ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}, DeletedAt: {DeletedAt}", serviceRequest.Id, user.Id, serviceRequest.DeletedAt);

        return Result.Success();
    }
}
