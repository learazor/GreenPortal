using Entities.model.user;
using Microsoft.EntityFrameworkCore;
using GreenPortal.model;

namespace GreenPortal.repository
{
    public class GreenPortalContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
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
            modelBuilder.Entity<Account>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<Admin>("Admin")
                .HasValue<Client>("Client")
                .HasValue<Company>("Company");

            modelBuilder.Entity<Account>()
                .HasKey(a => a.Email);

            modelBuilder.Entity<Company>()
                .HasOne<CompanyInfo>()
                .WithMany()
                .HasForeignKey(c => c.CompanyCode);
            
            modelBuilder.Entity<CompanyInstallation>()
                .HasKey(ci => new { ci.type, ci.company_code});

            modelBuilder.Entity<CompanyInfo>()
                .HasKey(ci => ci.company_code);
        }
    }
}