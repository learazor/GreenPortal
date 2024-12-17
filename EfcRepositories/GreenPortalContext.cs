using Entities.model.portal;
using Entities.model.user;
using GreenPortal.model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories
{
    public class GreenPortalContext : IdentityDbContext<User>
    {
        public DbSet<AdminUser> Admins { get; set; }
        public DbSet<ClientUser> Clients { get; set; }
        public DbSet<CompanyUser> Companies { get; set; }
        public DbSet<CompanyInstallation> companyinstallation { get; set; }
        public DbSet<CompanyInfo> companyinfo { get; set; }
        public DbSet<InstallationOrder> InstallationOrders { get; set; }


        public GreenPortalContext(DbContextOptions<GreenPortalContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TPH inheritance for Account
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<AdminUser>("Admin")
                .HasValue<ClientUser>("Client")
                .HasValue<CompanyUser>("Company");

            // Company relationship with CompanyInfo via CompanyCode
            modelBuilder.Entity<CompanyUser>()
                .HasOne<CompanyInfo>()
                .WithMany()
                .HasForeignKey(c => c.CompanyCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite key for CompanyInstallation
            modelBuilder.Entity<CompanyInstallation>()
                .HasKey(ci => new { ci.type, ci.company_code });

            // Primary key for CompanyInfo
            modelBuilder.Entity<CompanyInfo>()
                .HasKey(ci => ci.company_code);
        }
    }
}