using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequest;

public sealed class DeleteServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<DeleteServiceRequestCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await currentUserService.GetCurrentUserAsync();

        var serviceRequest = await unitOfWork.ServiceRequests
            .GetTableAsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        if (serviceRequest.CustomerId != user.Id)
            return Errors.Unauthorized;

        if (serviceRequest.Status == ServiceRequestStatus.Assigned)
            return Errors.ServiceAlreadyAccepted;

        serviceRequest.Status = ServiceRequestStatus.Cancelled;
        serviceRequest.IsDeleted = true;
        serviceRequest.DeletedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
