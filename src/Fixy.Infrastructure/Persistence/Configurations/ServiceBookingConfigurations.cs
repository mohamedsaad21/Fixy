using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public sealed class ServiceBookingConfigurations : IEntityTypeConfiguration<ServiceBooking>
{
    public void Configure(EntityTypeBuilder<ServiceBooking> builder)
    {
        builder.HasOne(x => x.Payment).WithOne(x => x.ServiceBooking).HasForeignKey<Payment>(x => x.ServiceBookingId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CustomerFeedback).WithOne(x => x.ServiceBooking).HasForeignKey<CustomerFeedback>(x => x.ServiceBookingId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TechnicianFeedback).WithOne(x => x.ServiceBooking).HasForeignKey<TechnicianFeedback>(x => x.ServiceBookingId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Status).HasConversion<string>();
    }
}
