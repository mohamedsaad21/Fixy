using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed record GetCustomerServiceRequestByIdQuery(Guid Id) : IRequest<Result<GetCustomerServiceRequestByIdResponse>>;
