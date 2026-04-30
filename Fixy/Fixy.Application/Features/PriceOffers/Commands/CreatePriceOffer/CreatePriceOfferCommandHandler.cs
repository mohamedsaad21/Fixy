using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Application.Resources;
using Fixy.Domain.Entities;
using Fixy.Domain.Enums;
using Fixy.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Application.Features.PriceOffers.Commands.CreatePriceOffer;

public sealed class CreatePriceOfferCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService) : IRequestHandler<CreatePriceOfferCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePriceOfferCommand request, CancellationToken cancellationToken)
    {
        var currentTechnician = await currentUserService.GetCurrentUserAsync();
        if (currentTechnician is not Technician technician)
            return Errors.Unauthorized;

        var serviceRequest = await unitOfWork.ServiceRequests.GetTableAsTracking().Include(x => x.PriceOffers)
            .Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == request.ServiceRequestId);
        if (serviceRequest == null)
            return Errors.ServiceRequestNotFound;

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
