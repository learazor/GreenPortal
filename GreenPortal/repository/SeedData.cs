using Entities.model.user;

namespace GreenPortal.repository;

using BCrypt.Net;

public static class SeedData
{
    public static void Initialize(GreenPortalContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Seed Admin account
        if (!context.Admins.Any())
        {
            var admin = new Admin
            {
                Email = "admin@greenportal.com",
                Password = BCrypt.HashPassword("admin") // Hash the password
            };

            context.Admins.Add(admin);
            context.SaveChanges();
        }
    }
}
