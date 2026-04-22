using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Commands.AddCategory;

public sealed record AddCategoryCommand(string NameEn, string NameAr, string Description) : IRequest<Result<Guid>>;