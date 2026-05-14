using Fixy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class TechnicianLocationConfiguration : IEntityTypeConfiguration<TechnicianLocation>
{
    public void Configure(EntityTypeBuilder<TechnicianLocation> builder)
    {
        builder.HasIndex(x => x.TechnicianId);
    }
}
