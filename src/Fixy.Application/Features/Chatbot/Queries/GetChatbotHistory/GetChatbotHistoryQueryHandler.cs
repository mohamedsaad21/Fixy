using AutoMapper;
using Fixy.Application.Bases;
using Fixy.Application.Contracts.Services;
using Fixy.Application.Wrappers;
using Fixy.Domain.Interfaces;
using MediatR;

namespace Fixy.Application.Features.Chatbot.Queries.GetChatbotHistory;

public sealed class GetChatbotHistoryQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetChatbotHistoryQuery, Result<PaginatedResult<GetChatbotHistoryResponse>>>
{
    public async Task<Result<PaginatedResult<GetChatbotHistoryResponse>>> Handle(GetChatbotHistoryQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();

        if (currentUser == null)
            return Errors.Unauthorized;

        var query = unitOfWork.ChatbotMessages.GetTableNoTracking()
            .Where(x => x.ChatbotConversation.UserId == currentUser.Id);

        var result = await query.Select(x => mapper.Map<GetChatbotHistoryResponse>(x)).ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return result;
    }
}
