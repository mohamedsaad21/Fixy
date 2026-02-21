using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceCategories.Queries.Results;
using MediatR;

namespace Fixy.Application.Features.ServiceCategories.Queries.Models;

public sealed record GetCategoriesListQuery : IRequest<Result<List<GetCategoriesListResponse>>>;
