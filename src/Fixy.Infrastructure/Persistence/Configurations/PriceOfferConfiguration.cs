using Fixy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class PriceOfferConfiguration : IEntityTypeConfiguration<PriceOffer>
{
    public void Configure(EntityTypeBuilder<PriceOffer> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.Status).HasConversion<string>();
        builder.HasIndex(x => x.ServiceRequestId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.TechnicianId);
    }
}
