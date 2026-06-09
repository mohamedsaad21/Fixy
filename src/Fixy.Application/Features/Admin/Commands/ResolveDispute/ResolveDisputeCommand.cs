using Fixy.Application.Bases;
using Fixy.Domain.Enums;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.ResolveDispute;

public sealed record ResolveDisputeCommand(Guid DisputeId, DisputeStatus Outcome, string ResolutionOutcome) : IRequest<Result>;
