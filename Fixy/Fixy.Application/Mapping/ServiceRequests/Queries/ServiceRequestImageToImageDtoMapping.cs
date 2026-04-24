using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById.Responses;
using Fixy.Domain.Entities;

namespace Fixy.Application.Mapping.ServiceRequests;

public partial class ServiceRequestProfile
{
    public void ServiceRequestImageToImageDtoMapping()
    {
        CreateMap<ServiceRequestImage, ImageDto>();
    }
}
