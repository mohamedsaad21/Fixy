using Fixy.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(x => x.BlockedByUser).WithMany().HasForeignKey(x => x.BlockedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Disputes).WithOne(x => x.Raiser).HasForeignKey(x => x.RaiserId).OnDelete(DeleteBehavior.Restrict);
    }
}
