using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceRequests.Commands.Models;

public record AddServiceRequestCommand(string Description, DateTime ScheduledDateTime) : IRequest<Result<Guid>>;