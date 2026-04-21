using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Mapping.PriceOffers.Commands;
using Fixy.Domain.Entities;
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
        await unitOfWork.SaveChangesAsync();

        var customer = serviceRequest.Customer;
        await notificationService.SendNotificationToUserAsync(customer.Id, new
        {
            type = "PRICE_OFFER_RECEIVED",
            message = $"You have received a new price offer of {priceOffer.Price} for your service request. Review and accept or reject it.",
            createdAt = DateTime.UtcNow
        });

        if (!string.IsNullOrEmpty(customer.FcmToken))
        {
            await notificationService.SendPushNotificationAsync(
                fcmToken: customer.FcmToken,
                title: "New Price Offer Received",
                body: $"You have received a new price offer of {priceOffer.Price} for your service request. Review and accept or reject it.",
                data: new Dictionary<string, string>
                {
                    { "type", "PRICE_OFFER_RECEIVED" },
                    { "priceOfferId", priceOffer.Id.ToString() },
                    { "serviceRequestId", serviceRequest.Id.ToString() },
                    { "price", priceOffer.Price.ToString() },
                    { "createdAt", DateTime.UtcNow.ToString("O") }
                }
            );
        }

        return priceOffer.Id;
    }
}
