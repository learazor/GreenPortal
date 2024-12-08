using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Entities.model.user;
using GreenPortal.model;

namespace GreenPortal.repository
{
    public class GreenPortalContext : IdentityDbContext<Account>
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyInstallation> companyinstallation { get; set; }
        public DbSet<CompanyInfo> companyinfo { get; set; }

        public GreenPortalContext(DbContextOptions<GreenPortalContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TPH inheritance for Account
            modelBuilder.Entity<Account>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<Admin>("Admin")
                .HasValue<Client>("Client")
                .HasValue<Company>("Company");

            // Company relationship with CompanyInfo via CompanyCode
            modelBuilder.Entity<Company>()
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