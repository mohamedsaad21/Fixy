using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Commands.BlockCustomer;

public sealed record BlockCustomerCommand
    (
        Guid CustomerId, 
        string Reason
    ) : IRequest<Result>;