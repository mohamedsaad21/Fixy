using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence.Repositories;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    private readonly DbSet<Conversation> _conversations;
    public ConversationRepository(FixyDbContext dbContext) : base(dbContext)
    {
        _conversations = dbContext.Set<Conversation>();
    }

    public async Task<Conversation> GetOrCreateAsync(Guid bookingId, Guid senderId, Guid receiverId)
    {
        var conversation = await _conversations.FirstOrDefaultAsync(c => c.ServiceBookingId == bookingId);

        if (conversation != null)
            return conversation;

        conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ServiceBookingId = bookingId,
            CustomerId = senderId,
            TechnicianId = receiverId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _conversations.AddAsync(conversation);
        return conversation;
    }

    public async Task CloseConversationAsync(Guid bookingId)
    {
        var conversation = await _conversations.SingleOrDefaultAsync(x => x.ServiceBookingId == bookingId);

        if (conversation != null)
            conversation.IsClosed = true;
    }
}
