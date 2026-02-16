using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public class ServiceBookingConfigurations : IEntityTypeConfiguration<ServiceBooking>
{
    public void Configure(EntityTypeBuilder<ServiceBooking> builder)
    {
        builder.HasOne(x => x.Payment).WithOne(x => x.ServiceBooking).HasForeignKey<Payment>(x => x.ServiceBookingId).OnDelete(DeleteBehavior.Restrict);
    }
}
