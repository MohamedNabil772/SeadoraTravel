using Microsoft.EntityFrameworkCore;
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
        await context.Database.MigrateAsync();

        // Seed Permissions
        var permissions = new List<Permission>
        {
            new Permission { Id = "Dashboard.View", Module = "Dashboard", Action = "View", DisplayName = "View Dashboard" },
            new Permission { Id = "Tours.View", Module = "Tours & Experiences", Action = "View", DisplayName = "View Tours" },
            new Permission { Id = "Tours.Create", Module = "Tours & Experiences", Action = "Create", DisplayName = "Create Tours" },
            new Permission { Id = "Tours.Edit", Module = "Tours & Experiences", Action = "Edit", DisplayName = "Edit Tours" },
            new Permission { Id = "Tours.Delete", Module = "Tours & Experiences", Action = "Delete", DisplayName = "Delete Tours" },
            new Permission { Id = "Bookings.View", Module = "Bookings & Vouchers", Action = "View", DisplayName = "View Bookings" },
            new Permission { Id = "Bookings.Create", Module = "Bookings & Vouchers", Action = "Create", DisplayName = "Create Bookings" },
            new Permission { Id = "Bookings.Edit", Module = "Bookings & Vouchers", Action = "Edit", DisplayName = "Edit Bookings" },
            new Permission { Id = "Inquiries.View", Module = "Inquiries & Concierge", Action = "View", DisplayName = "View Inquiries" },
            new Permission { Id = "AccessControl.ManageAccess", Module = "Users & Access", Action = "ManageAccess", DisplayName = "Manage Access" },
            new Permission { Id = "Settings.Edit", Module = "System Settings", Action = "Edit", DisplayName = "Edit Settings" }
        };

        foreach (var permission in permissions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Id == permission.Id))
            {
                context.Permissions.Add(permission);
            }
        }
        await context.SaveChangesAsync();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "SuperAdmin", "Admin", "OperationsManager", "ConciergeSpecialist", "Customer" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        // Assign all permissions to SuperAdmin
        var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
        if (superAdminRole != null)
        {
            var existingRolePerms = await context.RolePermissions.Where(rp => rp.RoleId == superAdminRole.Id).ToListAsync();
            var allPermissions = await context.Permissions.ToListAsync();
            foreach (var perm in allPermissions)
            {
                if (!existingRolePerms.Any(rp => rp.PermissionId == perm.Id))
                {
                    context.RolePermissions.Add(new RolePermission { RoleId = superAdminRole.Id, PermissionId = perm.Id });
                }
            }
            await context.SaveChangesAsync();
        }

        // Seed SuperAdmin
        var adminEmail = "admin@seadoratravel.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new User { UserName = adminEmail, Email = adminEmail, FirstName = "System", LastName = "Admin" };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "SuperAdmin");
        }

        var altAdminEmail = "admin@seadora.com";
        if (await userManager.FindByEmailAsync(altAdminEmail) == null)
        {
            var altAdmin = new User { UserName = altAdminEmail, Email = altAdminEmail, FirstName = "System", LastName = "Admin" };
            await userManager.CreateAsync(altAdmin, "Admin@123456");
            await userManager.AddToRoleAsync(altAdmin, "SuperAdmin");
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
