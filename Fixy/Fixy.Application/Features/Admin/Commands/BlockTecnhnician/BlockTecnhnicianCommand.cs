using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.BlockTecnhnician;

public sealed record BlockTecnhnicianCommand(Guid TechnicianId, string Reason) : IRequest<Result>;