using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IEncryptionProvider _encryptionProvider;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _encryptionProvider = new GenerateEncryptionProvider("BC205508E3ED4C42ACE5E2FE4B1B2431");
    }
    public virtual DbSet<Technician> Technicians { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.UseEncryption(_encryptionProvider);
    }
}
