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

        builder.HasMany(r => r.ServiceCategories).WithMany(c => c.ServiceRequests)
            .UsingEntity<ServiceRequestCategories>(
                 x => x.HasOne(rc => rc.Category).WithMany(c => c.ServiceRequestCategories)
                .HasForeignKey(rc => rc.CategoryId).OnDelete(DeleteBehavior.Restrict),
                x => x.HasOne(rc => rc.ServiceRequest).WithMany(r => r.ServiceRequestCategories)
                .HasForeignKey(rc => rc.ServiceRequestId),
                x => x.HasKey(rc => new { rc.CategoryId, rc.ServiceRequestId })
            );

        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(a => a.Country).HasMaxLength(100);
            a.Property(a => a.City).HasMaxLength(100);
            a.Property(a => a.Area).HasMaxLength(100);
            a.Property(a => a.Street).HasMaxLength(100);
            a.Property(a => a.BuildingNumber).HasMaxLength(100);
        });

        builder.HasMany(x => x.ServiceRequestImages).WithOne(x => x.ServiceRequest).HasForeignKey(x => x.ServiceRequestId);
        builder.HasMany(x => x.PriceOffers).WithOne(x => x.ServiceRequest).HasForeignKey(x => x.ServiceRequestId);
        builder.HasMany(x => x.ServiceBookings).WithOne(x => x.ServiceRequest).HasForeignKey(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
