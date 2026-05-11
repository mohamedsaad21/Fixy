using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.UnblockTechnician;

public sealed record UnblockTechnicianCommand(Guid TechnicianId) : IRequest<Result>;