using Fixy.Application.Bases;
using Fixy.Application.Common.DTOs.Payment;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fixy.Application.Features.Payments.Commands.ProcessCallback;

public record ProcessCallbackCommand(IQueryCollection QueryData) : IRequest<Result<bool>>;