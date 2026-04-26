using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Fixy.Domain.Entities;
using Fixy.Domain.Entities.Chat;
using Fixy.Domain.Entities.Feedback;
using Fixy.Domain.Entities.Identity;
using Fixy.Domain.Entities.Payments;
using Fixy.Domain.SP.TechnicianAvailableRequests;
using Fixy.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fixy.Infrastructure.Persistence;

public class FixyDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly IEncryptionProvider _encryptionProvider;
    public FixyDbContext(DbContextOptions<FixyDbContext> options) : base(options)
    {
        _encryptionProvider = new GenerateEncryptionProvider("BC205508E3ED4C42ACE5E2FE4B1B2431");
    }
    public virtual DbSet<Technician> Technicians { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
    public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
    public virtual DbSet<PriceOffer> PriceOffers { get; set; }
    public virtual DbSet<ServiceRequestCategories> ServiceRequestCategories { get; set; }
    public virtual DbSet<ServiceBooking> ServiceBookings { get; set; }
    public virtual DbSet<ServiceBookingImage> ServiceBookingImages { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }
    public virtual DbSet<TechnicianFeedback> TechnicianFeedbacks  { get; set; }
    public virtual DbSet<TechnicianCommissionOwed> TechnicianCommissionsOwed { get; set; }
    public virtual DbSet<Payout> Payouts { get; set; }
    public virtual DbSet<OtpCode> OtpCodes { get; set; }
    public virtual DbSet<Conversation> Conversations { get; set; }
    public virtual DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ServiceRequestSpResult> ServiceRequestSpResults { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(TechniciansConfigurations).Assembly);

        builder.UseEncryption(_encryptionProvider);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        // SP
        builder.Entity<ServiceRequestSpResult>().HasNoKey().ToView(null);

        builder.Entity<ApplicationUser>(options =>
        {
            options.UseTptMappingStrategy();
        });
    }
}
