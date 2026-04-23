using Fixy.Domain.Entities.Chat;

namespace Fixy.Domain.Interfaces;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation> GetOrCreateAsync(Guid bookingId, Guid senderId, Guid receiverId);
}
