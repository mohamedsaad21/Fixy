using Fixy.Application.Bases;
using Fixy.Application.Wrappers;
using MediatR;

namespace Fixy.Application.Features.Chatbot.Queries.GetChatbotHistory;

public sealed record GetChatbotHistoryQuery
    (
        int PageNumber,
        int PageSize
    ) : IRequest<Result<PaginatedResult<GetChatbotHistoryResponse>>>;