using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.RejectTechnician;

public sealed record RejectTechnicianCommand(Guid TechnicianId) : IRequest<Result>;