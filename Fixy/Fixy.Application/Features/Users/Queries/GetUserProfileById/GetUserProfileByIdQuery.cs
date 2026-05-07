using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Users.Queries.GetUserProfileById;

public sealed record GetUserProfileByIdQuery(Guid Id) : IRequest<Result<GetUserProfileByIdResponse>>;