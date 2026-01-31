using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Payments.Commands.HandleStripePaymentSucceeded;

public record HandleStripePaymentSucceededCommand() : IRequest<Result>;