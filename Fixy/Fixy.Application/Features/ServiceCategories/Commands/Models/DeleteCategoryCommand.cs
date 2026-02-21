using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.Models;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
