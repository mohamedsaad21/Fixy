using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategory.Commands.Models;

public record AddCategoryCommand(string Name, string Description) : IRequest<Result<Guid>>;