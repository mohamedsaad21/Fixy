using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public record GetServiceRequestByIdQuery(Guid Id) : IRequest<Result<GetServiceRequestByIdDto>>;
