using Fixy.Application.Bases;
using Fixy.Application.Features.Authentication.Queries.Results;
using MediatR;

namespace Fixy.Application.Features.Authentication.Queries.Models;

public class GetCustomersQuery : IRequest<Result<List<GetCustomersResponse>>>
{
}
