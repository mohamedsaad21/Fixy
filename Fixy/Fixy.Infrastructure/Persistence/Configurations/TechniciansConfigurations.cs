using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public class TechniciansConfigurations : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NationalId).IsRequired().HasMaxLength(14);
        builder.Property(x => x.YearsOfExperience).IsRequired();

        builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Technician>(x => x.Id);
    }
}
