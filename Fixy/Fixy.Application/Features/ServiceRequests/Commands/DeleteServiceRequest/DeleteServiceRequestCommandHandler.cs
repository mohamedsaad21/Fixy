using Fixy.Application.Bases;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequest;

public sealed class DeleteServiceRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteServiceRequestCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceRequestCommand request, CancellationToken cancellationToken)
    {
        // Get Serive Request
        var serviceRequest = await unitOfWork.ServiceRequests
            .GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

        //serviceRequest.IsDeleted = true;
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
