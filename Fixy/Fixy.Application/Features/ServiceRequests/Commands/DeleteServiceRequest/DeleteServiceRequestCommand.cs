using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequest;

public sealed record DeleteServiceRequestCommand(Guid Id) : IRequest<Result>;