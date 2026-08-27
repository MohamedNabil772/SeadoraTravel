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
            new Permission { Id = "Settings.Edit", Module = "System Settings", Action = "Edit", DisplayName = "Edit Settings" },
            new Permission { Id = "Finance.ViewDashboard", Module = "Finance", Action = "ViewDashboard", DisplayName = "View Financial Dashboard" },
            new Permission { Id = "Finance.ViewReports", Module = "Finance", Action = "ViewReports", DisplayName = "View Financial Reports" },
            new Permission { Id = "Finance.ManagePayments", Module = "Finance", Action = "ManagePayments", DisplayName = "Record & Manage Payments" },
            new Permission { Id = "Finance.PostAdjustments", Module = "Finance", Action = "PostAdjustments", DisplayName = "Post Financial Adjustments" },
            new Permission { Id = "Finance.Reconcile", Module = "Finance", Action = "Reconcile", DisplayName = "Reconcile Payments" },
            new Permission { Id = "Finance.Export", Module = "Finance", Action = "Export", DisplayName = "Export Financial Reports" }
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

        string[] roleNames = { "SuperAdmin", "Admin", "OperationsManager", "ConciergeSpecialist", "Accountant", "BusinessOwner", "Customer" };
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

        // Assign Finance permissions to Accountant (operate finance) and BusinessOwner (read-only insight).
        // ponytail: idempotent add-if-missing, same shape as SuperAdmin; admins can still re-tune via RolesView.
        await AssignPermissionsAsync(context, roleManager, "Accountant", new[]
        {
            "Dashboard.View", "Bookings.View",
            "Finance.ViewDashboard", "Finance.ViewReports", "Finance.ManagePayments",
            "Finance.PostAdjustments", "Finance.Reconcile", "Finance.Export"
        });
        await AssignPermissionsAsync(context, roleManager, "BusinessOwner", new[]
        {
            "Dashboard.View", "Finance.ViewDashboard", "Finance.ViewReports", "Finance.Export"
        });
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

        // Seed a demo Accountant and Business Owner so the Finance area is reachable out of the box.
        var accountantEmail = "accountant@seadoratravel.com";
        if (await userManager.FindByEmailAsync(accountantEmail) == null)
        {
            var accountant = new User { UserName = accountantEmail, Email = accountantEmail, FirstName = "Amina", LastName = "Accountant" };
            await userManager.CreateAsync(accountant, "Accountant123!");
            await userManager.AddToRoleAsync(accountant, "Accountant");
        }

        var ownerEmail = "owner@seadoratravel.com";
        if (await userManager.FindByEmailAsync(ownerEmail) == null)
        {
            var owner = new User { UserName = ownerEmail, Email = ownerEmail, FirstName = "Omar", LastName = "Owner" };
            await userManager.CreateAsync(owner, "Owner123!");
            await userManager.AddToRoleAsync(owner, "BusinessOwner");
        }
    }

    // ponytail: idempotent role->permission assignment; skips silently if the role or a permission id is missing.
    private static async Task AssignPermissionsAsync(SeadoraIdentityDbContext context, RoleManager<Role> roleManager, string roleName, string[] permissionIds)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null) return;

        var existing = await context.RolePermissions.Where(rp => rp.RoleId == role.Id).Select(rp => rp.PermissionId).ToListAsync();
        var valid = await context.Permissions.Where(p => permissionIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();

        foreach (var permId in valid)
        {
            if (!existing.Contains(permId))
            {
                context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permId });
            }
        }
        await context.SaveChangesAsync();
    }
}
