using Fixy.Application.Bases;
using MediatR;

namespace Fixy.Application.Features.Admin.Queries.GetUserInfoById;

public sealed record GetUserInfoByIdQuery(Guid UserId) : IRequest<Result<GetUserInfoByIdResponse>>;