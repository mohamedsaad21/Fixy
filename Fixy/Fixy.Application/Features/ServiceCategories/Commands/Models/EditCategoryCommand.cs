using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.Models;

public sealed record EditCategoryCommand(Guid Id, string Name, string Description) : IRequest<Result>;