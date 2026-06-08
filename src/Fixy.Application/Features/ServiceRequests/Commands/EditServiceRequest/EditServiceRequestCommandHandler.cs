using Fixy.Application.Bases;
using Fixy.Application.Contracts.ExternalServices;
using Fixy.Application.Mapping.ServiceRequests.Commands;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;

public sealed class EditServiceRequestCommandHandler(IUnitOfWork unitOfWork, IStorageService fileService, ILogger<EditServiceRequestCommandHandler> logger) : IRequestHandler<EditServiceRequestCommand, Result>
{
    public async Task<Result> Handle(EditServiceRequestCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Customer attempting to edit service request. ServiceRequestId: {ServiceRequestId}", request.Id);

        var serviceRequest = await unitOfWork.ServiceRequests.GetTableAsTracking()
            .Include(x => x.ServiceCategories).Include(x => x.PriceOffers).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (serviceRequest == null)
        {
            logger.LogWarning("Service request edit failed — service request not found. ServiceRequestId: {ServiceRequestId}", request.Id);
            return Errors.ServiceRequestNotFound;
        }

        if (serviceRequest.PriceOffers.Any())
        {
            logger.LogWarning("Service request edit failed — cannot edit with active price offers. ServiceRequestId: {ServiceRequestId}, ActiveOffersCount: {ActiveOffersCount}", request.Id, serviceRequest.PriceOffers.Count);
            return Errors.CannotEditWithActiveOffers;
        }

        // Load only the categories that were submitted
        var newCategories = await unitOfWork.ServiceCategories
            .GetTableAsTracking()
            .Where(c => request.ServiceCategories.Contains(c.Id))
            .ToListAsync(cancellationToken);

        serviceRequest = request.ToServiceRequestDomain(serviceRequest, newCategories);
        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("Service request updated successfully. ServiceRequestId: {ServiceRequestId}", request.Id);
        return Result.Success();
    }
}
