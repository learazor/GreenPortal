using GreenPortal.model;

namespace GreenPortal.db;

using Microsoft.EntityFrameworkCore;

public class CompanyDbContext : DbContext
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options) { }

    public DbSet<CompanyInstallation> CompanyInstallations { get; set; }
    public DbSet<CompanyInfo> CompanyInfos { get; set; }
}