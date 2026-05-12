using Fixy.Application.Bases;
using Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt.Responses;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.ConfirmCashReceipt;

public sealed record ConfirmCashReceiptCommand(Guid BookingId) : IRequest<Result<ConfirmCashReceiptResponse>>;
