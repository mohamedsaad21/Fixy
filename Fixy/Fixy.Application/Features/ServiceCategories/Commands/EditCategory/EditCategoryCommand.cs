using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.EditCategory;

public sealed record EditCategoryCommand(Guid Id, string Name, string Description) : IRequest<Result>;