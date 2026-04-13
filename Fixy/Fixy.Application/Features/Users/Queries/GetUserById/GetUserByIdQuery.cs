using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserByIdResponse>>;