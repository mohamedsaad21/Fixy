using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public sealed record CreateServiceRequestCommand
    (
        string Description, 
        IFormFile[]? Images,
        DateTime ScheduledDateTime, 
        Guid[] ServiceCategoriesIds,
        AddressDto Address
    ) : IRequest<Result<Guid>>;