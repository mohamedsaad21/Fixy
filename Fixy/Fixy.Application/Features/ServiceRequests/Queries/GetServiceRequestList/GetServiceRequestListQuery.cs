using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;

public record GetServiceRequestListQuery(): IRequest<Result<List<GetServiceRequestListDto>>>;
