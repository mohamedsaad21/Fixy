using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Queries.GetCategoriesList;

public sealed record GetCategoriesListQuery : IRequest<Result<List<GetCategoriesListResponse>>>;
