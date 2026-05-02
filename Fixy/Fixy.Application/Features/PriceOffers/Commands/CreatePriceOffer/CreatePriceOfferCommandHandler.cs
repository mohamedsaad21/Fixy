using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Application.Resources;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.CreatePriceOffer;

public sealed class CreatePriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, 
    INotificationService notificationService) : IRequestHandler<CreatePriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePriceOfferCommand request, CancellationToken cancellationToken)
    {
        var currentTechnicianId = currentUserService.GetCurrentUserId();

        var technician = await unitOfWork.Technicians.GetTableNoTracking()
            .FirstOrDefaultAsync(x => x.Id == currentTechnicianId);

        if (technician == null)
            return Errors.Unauthorized;

        var serviceRequest = await unitOfWork.ServiceRequests.GetTableAsTracking().Include(x => x.PriceOffers)
            .Include(x => x.Customer).Include(x => x.BlockedServiceRequests).FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);

        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;
        // Check that the status is still pending
        if (serviceRequest.Status != ServiceRequestStatus.Pending)
            return Errors.ServiceRequestUnavailable;

        // Check that technician cannot send price offer to a service request of previous cancelled booking
        if (serviceRequest.BlockedServiceRequests.Any(x => x.ServiceRequestId == serviceRequest.Id && x.TechnicianId == technician.Id))
            return Errors.BlockedTechnicianOffer;
       
        // Check that technician haven't sent price offer before
        if (serviceRequest.PriceOffers.Any(x => x.TechnicianId == technician.Id))
            return Errors.AlreadyCreatedPriceOffer;

        var priceOffer = request.ToPriceOfferDomain();
        priceOffer.TechnicianId = technician.Id;

        serviceRequest.PriceOffers.Add(priceOffer);

        var customer = serviceRequest.Customer;

        await notificationService.SendFullNotificationAsync(
            customer,
            NotificationType.PriceOfferReceived,
            SharedResourcesKeys.NotificationPriceOfferReceivedTitle,
            SharedResourcesKeys.NotificationPriceOfferReceivedBody
        );
        await unitOfWork.SaveChangesAsync();
        return priceOffer.Id;
    }
}
