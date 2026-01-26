using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public record GetMyRequestsQuery() : IRequest<Result<List<GetServiceRequestListDto>>>;