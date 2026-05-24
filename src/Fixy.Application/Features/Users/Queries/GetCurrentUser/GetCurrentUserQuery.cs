using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery() : IRequest<Result<GetCurrentUserResponse>>;