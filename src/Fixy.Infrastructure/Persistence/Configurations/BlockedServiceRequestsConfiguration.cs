using Fixy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class BlockedServiceRequestsConfiguration : IEntityTypeConfiguration<BlockedServiceRequest>
{
    public void Configure(EntityTypeBuilder<BlockedServiceRequest> builder)
    {
        builder.HasOne(x => x.ServiceRequest).WithMany(x => x.BlockedServiceRequests).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Technician).WithMany(x => x.BlockedServiceRequests).OnDelete(DeleteBehavior.Restrict);
    }
}
