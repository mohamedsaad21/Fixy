using Fixy.Application.Common.DTOs;
using Fixy.Domain.Enums;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById.Responses;

public class GetCustomerServiceRequestByIdResponse
{
    public Guid Id { get; set; }
    public string CustomerUserName { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public List<string> ServiceCategories { get; set; }
    public AddressDto Address { get; set; }
    public ServiceRequestStatus Status { get; set; }
    public List<PriceOfferDto> PriceOffers { get; set; }
    public List<ImageDto> Images { get; set; }
}
