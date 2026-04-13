using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.PayCommission.Responses;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.PayCommission;

public record PayCommissionCommand
    (
        string TechnicianName,
        string TechnicianEmail,
        string TechnicianPhone
    ) : IRequest<Result<PayCommissionResponse>>;