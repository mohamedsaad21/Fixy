using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public sealed record CancelServiceRequestCommand(Guid Id) : IRequest<Result>;