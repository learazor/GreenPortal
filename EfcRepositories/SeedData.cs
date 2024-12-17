namespace GreenPortal.repository;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Entities.model.user;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure admin role exists
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }

        // Seed admin user
        if (userManager.FindByEmailAsync("admin@greenportal.com").Result == null)
        {
            var admin = new AdminUser
            {
                UserName = "admin@greenportal.com",
                Email = "admin@greenportal.com",
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(admin, "Administrator12#").Result;
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(admin, "Admin").Wait();
            }
        }
    }
}

