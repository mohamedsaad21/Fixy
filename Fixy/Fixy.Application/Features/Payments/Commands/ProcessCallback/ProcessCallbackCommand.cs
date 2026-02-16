using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs.Payment;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public record ProcessCallbackCommand(PaymobCallbackDto Callback) : IRequest<Result<bool>>;