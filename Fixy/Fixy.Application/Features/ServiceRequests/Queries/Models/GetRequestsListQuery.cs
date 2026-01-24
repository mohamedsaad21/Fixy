using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceRequests.Queries.Results;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Queries.Models;

public record GetRequestsListQuery(): IRequest<Result<List<GetRequestsListResponse>>>;
