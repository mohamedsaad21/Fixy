using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.GetMyRequests;

public sealed record GetMyRequestsQuery() : IRequest<Result<List<GetServiceRequestListDto>>>;