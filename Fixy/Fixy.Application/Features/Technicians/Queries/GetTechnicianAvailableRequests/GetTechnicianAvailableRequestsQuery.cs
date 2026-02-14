using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs;
using MediatR;

namespace Fixy.Application.Features.Technicians.Queries.GetTechnicianAvailableRequests;

public record GetTechnicianAvailableRequestsQuery() : IRequest<Result<List<GetServiceRequestListDto>>>;
