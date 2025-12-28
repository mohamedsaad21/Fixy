using Fixy.Application.Bases;
using Fixy.Application.Features.ServiceCategory.Queries.Results;
using MediatR;

namespace Fixy.Application.Features.ServiceCategory.Queries.Models;

public record GetCategoriesListQuery : IRequest<Result<List<GetCategoriesListResponse>>>;
