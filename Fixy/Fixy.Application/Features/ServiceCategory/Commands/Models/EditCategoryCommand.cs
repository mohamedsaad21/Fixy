using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategory.Commands.Models;

public record EditCategoryCommand(Guid Id, string Name, string Description) : IRequest<Result>;