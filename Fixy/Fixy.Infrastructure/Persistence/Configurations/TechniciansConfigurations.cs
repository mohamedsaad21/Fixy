using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public class TechniciansConfigurations : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.Property(x => x.NationalId).HasMaxLength(14);
        builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Technician>(x => x.Id);
        builder.HasOne(x => x.ServiceCategory).WithMany(x => x.Technicians).HasForeignKey(x => x.ServiceCategoryId);
        builder.HasMany(x => x.ServiceBookings).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TechnicianLocation).WithOne(x => x.Technician).HasForeignKey<TechnicianLocation>(x => x.TechnicianId);
        builder.HasMany(x => x.PriceOffers).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.TechnicianFeedbacks).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.CustomerFeedbacks).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Payouts).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.TechnicianCommissionsOwed).WithOne(x => x.Technician).HasForeignKey(x => x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
    }
}
