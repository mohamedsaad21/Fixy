using Fixy.Application.Features.Chatbot.Queries.GetChatbotHistory;
using Fixy.Domain.Entities.Chatbot;

namespace Fixy.Application.Mapping.Chatbot;

public partial class ChatbotProfile
{
    public void GetChatbotMessageDomainToGetChatbotHistoryResponseMapping()
    {
        CreateMap<ChatbotMessage, GetChatbotHistoryResponse>();
    }
}
