using Fixy.Application.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.ServiceRequests.Commands.AddServiceRequestImages;

public sealed record AddServiceRequestImagesCommand
    (
        Guid ServiceRequestId,
        IFormFile[] Images
    ) : IRequest<Result>;
