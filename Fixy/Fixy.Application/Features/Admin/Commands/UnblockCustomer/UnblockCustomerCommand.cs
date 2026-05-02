using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.UnblockCustomer;

public sealed record UnblockCustomerCommand(Guid CustomerId) : IRequest<Result>;