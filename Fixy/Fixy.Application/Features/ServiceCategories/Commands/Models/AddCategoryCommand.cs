using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.Models;

public record AddCategoryCommand(string Name, string Description) : IRequest<Result<Guid>>;