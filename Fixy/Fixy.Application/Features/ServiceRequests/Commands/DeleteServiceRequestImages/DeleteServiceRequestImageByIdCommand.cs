using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.DeleteServiceRequestImages;

public record DeleteServiceRequestImageByIdCommand(Guid ImageId) : IRequest<Result>;
