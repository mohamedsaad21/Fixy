using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public sealed class CancelServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<CancelServiceRequestCommand, Result>
{
    public async Task<Result> Handle(CancelServiceRequestCommand request, CancellationToken cancellationToken)
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

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
