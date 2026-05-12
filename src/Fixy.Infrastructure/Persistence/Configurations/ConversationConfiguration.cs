using Fixy.Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasOne(c => c.Customer)
        .WithMany(u => u.CustomerConversations)
        .HasForeignKey(c => c.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Technician)
            .WithMany(u => u.TechnicianConversations)
            .HasForeignKey(c => c.TechnicianId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
