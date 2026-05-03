using Fixy.Application.Common.DTOs;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById.Responses;

namespace Fixy.Application.Features.Technicians.Queries.GetServiceRequestById;

public class GetTechnicianServiceRequestByIdResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerUserName { get; set; }
    public string Description { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public List<string> ServiceCategories { get; set; }
    public AddressDto Address { get; set; }
    public string Status { get; set; }
    public List<ImageDto> Images { get; set; }
}
