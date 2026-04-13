using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetServiceRequestList;

public sealed record GetServiceRequestListQuery(): IRequest<Result<List<GetServiceRequestListDto>>>;
