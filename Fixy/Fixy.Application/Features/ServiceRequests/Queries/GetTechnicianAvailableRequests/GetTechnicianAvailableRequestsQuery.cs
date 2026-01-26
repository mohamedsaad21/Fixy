using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetTechnicianAvailableRequests;

public record GetTechnicianAvailableRequestsQuery() : IRequest<Result<List<GetServiceRequestListDto>>>;
