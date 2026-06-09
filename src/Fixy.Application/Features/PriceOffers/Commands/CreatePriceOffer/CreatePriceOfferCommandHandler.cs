using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fixy.Application.Features.PriceOffers.Commands.CreatePriceOffer;

public sealed class CreatePriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<CreatePriceOfferCommandHandler> logger) : IRequestHandler<CreatePriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePriceOfferCommand request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = await currentUserService.GetCurrentUserId();
        logger.LogInformation("Technician attempting to create price offer. TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}, OfferedPrice: {OfferedPrice}", currentTechnicianId, request.ServiceRequestId, request.Price);
        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
        {
            logger.LogWarning("Price offer creation failed — technician not found. TechnicianId: {TechnicianId}", currentTechnicianId);
            return Errors.Unauthorized;
        }

        var serviceRequest = await unitOfWork.ServiceRequests.GetTableAsTracking().Include(x => x.PriceOffers)
            .Include(x => x.Customer).Include(x => x.BlockedServiceRequests).FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);

        if (serviceRequest == null)
        {
            logger.LogWarning("Price offer creation failed — service request not found. TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}", technician.Id, request.ServiceRequestId);
            return Errors.ServiceRequestNotFound;
        }
        // Check that the status is still pending
        if (serviceRequest.Status != ServiceRequestStatus.Pending)
        {
            logger.LogWarning("Price offer creation failed — service request is no longer available. TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}, CurrentStatus: {CurrentStatus}", technician.Id, request.ServiceRequestId, serviceRequest.Status);
            return Errors.ServiceRequestUnavailable;
        }

        // Check that technician cannot send price offer to a service request of previous cancelled booking
        if (serviceRequest.BlockedServiceRequests.Any(x => x.ServiceRequestId == serviceRequest.Id && x.TechnicianId == technician.Id))
        {
            logger.LogWarning("Price offer creation failed — technician is blocked from this service request due to prior cancellation. TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}", technician.Id, request.ServiceRequestId);
            return Errors.BlockedTechnicianOffer;
        }
    
        // Check that technician haven't sent price offer before
        if (serviceRequest.PriceOffers.Any(x => x.TechnicianId == technician.Id))
        {
            logger.LogWarning("Price offer creation failed — technician has already submitted an offer. TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}", technician.Id, request.ServiceRequestId);
            return Errors.AlreadyCreatedPriceOffer;
        }

        var priceOffer = request.ToPriceOfferDomain();
        priceOffer.TechnicianId = technician.Id;

        serviceRequest.PriceOffers.Add(priceOffer);

        var customer = serviceRequest.Customer;

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Price offer created successfully. PriceOfferId: {PriceOfferId}, TechnicianId: {TechnicianId}, ServiceRequestId: {ServiceRequestId}, CustomerId: {CustomerId}, OfferedPrice: {OfferedPrice}",
            priceOffer.Id, technician.Id, serviceRequest.Id, serviceRequest.Customer.Id, priceOffer.Price);

        BackgroundJob.Enqueue<INotificationService>(x => x.SendFullNotificationAsync(
            customer,
            NotificationType.PriceOfferReceived,
            SharedResourcesKeys.NotificationPriceOfferReceivedTitle,
            SharedResourcesKeys.NotificationPriceOfferReceivedBody
        ));

        return priceOffer.Id;
    }
}
