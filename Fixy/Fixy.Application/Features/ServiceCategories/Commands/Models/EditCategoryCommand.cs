using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.Models;

public record EditCategoryCommand(Guid Id, string Name, string Description) : IRequest<Result>;