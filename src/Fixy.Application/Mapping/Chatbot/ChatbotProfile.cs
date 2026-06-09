using AutoMapper;

namespace Fixy.Application.Mapping.Chatbot;

public partial class ChatbotProfile : Profile
{
    public ChatbotProfile()
    {
        GetChatbotMessageDomainToGetChatbotHistoryResponseMapping();
    }
}
