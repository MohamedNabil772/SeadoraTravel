using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Seadora.Identity.Domain.Entities;
using Seadora.Identity.Infrastructure.Persistence;

namespace Seadora.Identity.Infrastructure.Persistence;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<SeadoraIdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "BookingManager", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        // Seed Admin
        var adminEmail = "admin@seadoratravel.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User { UserName = adminEmail, Email = adminEmail, FirstName = "System", LastName = "Admin" };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed Booking Manager
        var managerEmail = "manager@seadoratravel.com";
        if (await userManager.FindByEmailAsync(managerEmail) == null)
        {
            var manager = new User { UserName = managerEmail, Email = managerEmail, FirstName = "Booking", LastName = "Manager" };
            await userManager.CreateAsync(manager, "Manager123!");
            await userManager.AddToRoleAsync(manager, "BookingManager");
        }

        // Seed Customer
        var customerEmail = "customer@gmail.com";
        if (await userManager.FindByEmailAsync(customerEmail) == null)
        {
            var customer = new User { UserName = customerEmail, Email = customerEmail, FirstName = "John", LastName = "Doe" };
            await userManager.CreateAsync(customer, "Customer123!");
            await userManager.AddToRoleAsync(customer, "Customer");
        }
    }
}
