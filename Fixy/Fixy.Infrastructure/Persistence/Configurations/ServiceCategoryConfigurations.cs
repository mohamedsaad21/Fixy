using Fixy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fixy.Infrastructure.Persistence.Configurations;

public class ServiceCategoryConfigurations : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.HasData(
            new
            {
                Id = Guid.Parse("8C048174-8ABA-4366-95C6-88969A6B0694"),
                NameEn = "Electrical",
                NameAr = "أعمال كهربائية",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            },
            new
            {
                Id = Guid.Parse("1F598BC0-E473-4161-B1AF-6B8670AA7FB5"),
                NameEn = "Carpentry Repair",
                NameAr = "تصليح نجارة",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            },
            new
            {
                Id = Guid.Parse("CF6BCBBC-EE19-4D10-B8C9-1694C4AE4DF1"),
                NameEn = "Air Conditioning",
                NameAr = "تكييف وتبريد",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            },
            new
            {
                Id = Guid.Parse("AC6134F9-E2E0-4132-9B5C-C86E76613082"),
                NameEn = "Painting and Wall Repair",
                NameAr = "دهانات وإصلاح جدران",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            },
            new
            {
                Id = Guid.Parse("C695A1A3-C32C-462A-824B-B2057525CC2C"),
                NameEn = "Appliance Repair",
                NameAr = "تصليح أجهزة منزلية",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            },
            new
            {
                Id = Guid.Parse("5379B080-90CE-416A-B454-9A343F08F0FC"),
                NameEn = "Cleaning Services",
                NameAr = "خدمات تنظيف",
                Description = "",
                CreatedAt = DateTime.Parse("2026-10-05")
            }
);
    }
}
