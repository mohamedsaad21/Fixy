using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.Models;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
