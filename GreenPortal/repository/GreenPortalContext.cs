using Microsoft.EntityFrameworkCore;
using GreenPortal.model;

namespace GreenPortal.repository
{
    public class GreenPortalContext : DbContext
    {
        public DbSet<CompanyInstallation> companyinstallation { get; set; }
        public DbSet<CompanyInfo> companyinfo { get; set; }

        public GreenPortalContext(DbContextOptions<GreenPortalContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for CompanyInstallation
            modelBuilder.Entity<CompanyInstallation>()
                .HasKey(ci => new { ci.type, ci.company_code});

            // Configure CompanyInfo with CompanyCode as the primary key
            modelBuilder.Entity<CompanyInfo>()
                .HasKey(ci => ci.company_code);
        }
    }
}