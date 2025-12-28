using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategory.Commands.Models;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
