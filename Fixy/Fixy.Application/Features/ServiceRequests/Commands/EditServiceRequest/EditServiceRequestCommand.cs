using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.EditServiceRequest;

public record EditServiceRequestCommand(
        Guid Id,
        string Description,
        DateTime ScheduledDateTime,
        AddressDto Address
    ) : IRequest<Result>;