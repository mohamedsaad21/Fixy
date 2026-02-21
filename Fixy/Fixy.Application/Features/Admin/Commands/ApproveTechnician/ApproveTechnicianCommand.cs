using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.ApproveTechnician;

public sealed record ApproveTechnicianCommand(Guid TechnicianId) : IRequest<Result>;