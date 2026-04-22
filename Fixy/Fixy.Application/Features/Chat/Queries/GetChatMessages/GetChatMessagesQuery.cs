using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using MediatR;

namespace Fixy.Application.Features.Chat.Queries.GetChatMessages;

public sealed record GetChatMessagesQuery
    (
        Guid BookingId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<PaginatedResult<GetChatMessagesResponse>>>;