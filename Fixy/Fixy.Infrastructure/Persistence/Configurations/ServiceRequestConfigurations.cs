using Fixy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public class ServiceRequestConfigurations : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ScheduledDateTime).IsRequired();
        builder.Property(x => x.Description).IsRequired();

        builder.HasOne(x => x.Customer).WithMany(x => x.ServiceRequests).HasForeignKey(x => x.CustomerId);
    }
}
