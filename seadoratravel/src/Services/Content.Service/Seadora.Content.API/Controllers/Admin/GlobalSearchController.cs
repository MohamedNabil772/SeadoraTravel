using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.API.Controllers.Admin;

[ApiController]
[Route("api/admin/search")]
[Route("api/search")]
public class GlobalSearchController : ControllerBase
{
    private readonly IContentDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public GlobalSearchController(IContentDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string? q)
    {
        var query = q?.Trim().ToLowerInvariant() ?? string.Empty;
        if (query.Length > 100) query = query.Substring(0, 100);

        var result = new GlobalSearchResultDto();

        // 1. Navigation / Quick Actions
        var navItems = GetSystemNavItems();
        if (string.IsNullOrWhiteSpace(query))
        {
            result.QuickActions = navItems.Take(6).ToList();
            return Ok(result);
        }
        else
        {
            result.QuickActions = navItems
                .Where(n => n.Title.ToLowerInvariant().Contains(query) || n.Keywords.ToLowerInvariant().Contains(query))
                .ToList();
        }

        // 2. Search Tours
        var allTours = await _context.Tours
            .AsNoTracking()
            .Select(t => new { t.Id, t.Names, t.Badge, t.Duration, t.Price, t.Currency, t.Emoji, CategoryNames = t.Category != null ? t.Category.Names : null, DestinationNames = t.Destination != null ? t.Destination.Names : null })
            .ToListAsync();

        result.Tours = allTours
            .Where(t => 
                (t.Names != null && t.Names.Values.Any(v => v.ToLowerInvariant().Contains(query))) ||
                (t.Badge != null && t.Badge.ToLowerInvariant().Contains(query)) ||
                (t.Duration != null && t.Duration.ToLowerInvariant().Contains(query)) ||
                (t.CategoryNames != null && t.CategoryNames.Values.Any(v => v.ToLowerInvariant().Contains(query))) ||
                (t.DestinationNames != null && t.DestinationNames.Values.Any(v => v.ToLowerInvariant().Contains(query)))
            )
            .Take(8)
            .Select(t => new SearchItemDto
            {
                Id = t.Id.ToString(),
                Title = t.Names?.GetValueOrDefault("en") ?? t.Names?.Values.FirstOrDefault() ?? "Untitled Tour",
                Subtitle = $"{t.DestinationNames?.GetValueOrDefault("en") ?? "Global"} • {t.Price} {t.Currency} • {t.Duration}",
                Category = "Tours",
                Icon = t.Emoji ?? "⛵",
                Badge = t.Badge,
                Route = $"/tours/{t.Id}/edit"
            })
            .ToList();

        // 3. Search Categories
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(c => new { c.Id, c.Names, c.IconName })
            .ToListAsync();

        result.Categories = categories
            .Where(c => c.Names != null && c.Names.Values.Any(v => v.ToLowerInvariant().Contains(query)))
            .Take(5)
            .Select(c => new SearchItemDto
            {
                Id = c.Id.ToString(),
                Title = c.Names?.GetValueOrDefault("en") ?? c.Names?.Values.FirstOrDefault() ?? "Category",
                Subtitle = "Experience Category",
                Category = "Categories",
                Icon = string.IsNullOrWhiteSpace(c.IconName) ? "🏷️" : c.IconName,
                Route = "/categories"
            })
            .ToList();

        // 4. Search Destinations
        var destinations = await _context.Destinations
            .AsNoTracking()
            .Select(d => new { d.Id, d.Names, d.FlagEmoji })
            .ToListAsync();

        result.Destinations = destinations
            .Where(d => d.Names != null && d.Names.Values.Any(v => v.ToLowerInvariant().Contains(query)))
            .Take(5)
            .Select(d => new SearchItemDto
            {
                Id = d.Id.ToString(),
                Title = d.Names?.GetValueOrDefault("en") ?? d.Names?.Values.FirstOrDefault() ?? "Destination",
                Subtitle = "Travel Destination",
                Category = "Destinations",
                Icon = d.FlagEmoji ?? "🗺️",
                Route = "/destinations"
            })
            .ToList();

        // 5. Search Tour Types
        var tourTypes = await _context.TourTypes
            .AsNoTracking()
            .Select(tt => new { tt.Id, tt.Names, tt.Code, tt.Icon })
            .ToListAsync();

        result.TourTypes = tourTypes
            .Where(tt => 
                (tt.Code != null && tt.Code.ToLowerInvariant().Contains(query)) ||
                (tt.Names != null && tt.Names.Values.Any(v => v.ToLowerInvariant().Contains(query)))
            )
            .Take(4)
            .Select(tt => new SearchItemDto
            {
                Id = tt.Id.ToString(),
                Title = tt.Names?.GetValueOrDefault("en") ?? tt.Code,
                Subtitle = $"Format Key: {tt.Code}",
                Category = "Trip Types",
                Icon = tt.Icon ?? "✨",
                Route = "/tour-types"
            })
            .ToList();

        return Ok(result);
    }

    private static List<SearchItemDto> GetSystemNavItems()
    {
        return new List<SearchItemDto>
        {
            // === QUICK ACTIONS ===
            new SearchItemDto { Id = "nav-create-tour", Title = "Create New Tour", Subtitle = "Add a luxury tour or excursion", Category = "Quick Actions", Icon = "➕", Route = "/tours/create", Keywords = "new add tour create experience package build" },

            // === ACCOUNT & PROFILE (password, email, profile, account, settings) ===
            new SearchItemDto { Id = "nav-profile", Title = "My Profile Settings", Subtitle = "Update your name, password, and personal details", Category = "Account", Icon = "👤", Route = "/profile", Keywords = "profile password change password reset password my account personal details name phone avatar update profile edit profile my settings account settings" },
            new SearchItemDto { Id = "nav-users-password", Title = "Admin Users & Passwords", Subtitle = "Manage user accounts, reset passwords, assign roles", Category = "Account", Icon = "👥", Route = "/users", Keywords = "password reset user password admin password users accounts staff team members credentials login email change email user email manage users create user delete user suspend user activate user" },

            // === EXPERIENCE MANAGEMENT ===
            new SearchItemDto { Id = "nav-tours", Title = "All Tours & Experiences", Subtitle = "View and manage the full catalog", Category = "Experience Management", Icon = "⛵", Route = "/tours", Keywords = "tours excursions list grid catalog experiences all tours view tours search tours filter tours" },
            new SearchItemDto { Id = "nav-destinations", Title = "Destinations Management", Subtitle = "Countries, cities, and ports of call", Category = "Experience Management", Icon = "🗺️", Route = "/destinations", Keywords = "destinations locations countries ports cities regions geography map places hurghada cairo luxor" },
            new SearchItemDto { Id = "nav-categories", Title = "Categories Management", Subtitle = "Experience activity classifications", Category = "Experience Management", Icon = "🏷️", Route = "/categories", Keywords = "categories tags activity types classification diving safari culture boat water sports adventure" },
            new SearchItemDto { Id = "nav-tour-types", Title = "Tour & Trip Types", Subtitle = "Group, Private, VIP, Yacht format configurations", Category = "Experience Management", Icon = "✨", Route = "/tour-types", Keywords = "types format group private yacht vip shore excursion multi-day classification trip type tour type" },

            // === CUSTOMER CARE & INQUIRIES ===
            new SearchItemDto { Id = "nav-bookings", Title = "Manage Bookings & Vouchers", Subtitle = "Guest reservations, payments, and voucher status", Category = "Customer Care", Icon = "📅", Route = "/bookings", Keywords = "bookings reservations guests vouchers orders payments invoices confirmation booking status pending confirmed cancelled refund" },
            new SearchItemDto { Id = "nav-inquiries", Title = "VIP Inquiries & Contact Requests", Subtitle = "Client messages, custom quotes, and concierge requests", Category = "Customer Care", Icon = "✉️", Route = "/inquiries", Keywords = "inquiries messages contacts leads concierge requests quotes custom quote contact form customer support help" },
            new SearchItemDto { Id = "nav-feedback", Title = "Customer Feedback & Reviews", Subtitle = "Guest reviews, ratings, and satisfaction metrics", Category = "Customer Care", Icon = "⭐", Route = "/feedback", Keywords = "feedback reviews ratings stars customer satisfaction complaints compliments guest reviews" },
            new SearchItemDto { Id = "nav-suppliers", Title = "Suppliers & Partners", Subtitle = "Tour operators, transport providers, and vendor contracts", Category = "Customer Care", Icon = "🤝", Route = "/suppliers", Keywords = "suppliers partners vendors operators transport providers contracts agreements commissions" },

            // === ADMINISTRATION & SECURITY ===
            new SearchItemDto { Id = "nav-roles", Title = "Roles & Permissions (RBAC)", Subtitle = "Access control matrix, role assignments, module privileges", Category = "Administration", Icon = "🛡️", Route = "/roles", Keywords = "roles permissions access control rbac security privileges modules admin booking manager content editor authorization restrict" },
            new SearchItemDto { Id = "nav-users", Title = "Admin Users", Subtitle = "Manage staff accounts, roles, and credentials", Category = "Administration", Icon = "👥", Route = "/users", Keywords = "users admin staff team members accounts employee invite user" },

            // === SYSTEM SETTINGS ===
            new SearchItemDto { Id = "nav-languages", Title = "Languages & Translations", Subtitle = "Multilingual localization dictionary and locale settings", Category = "System Settings", Icon = "🌐", Route = "/settings/languages", Keywords = "languages translations localization i18n arabic english german russian italian french locale multilingual translate" },
            new SearchItemDto { Id = "nav-currencies", Title = "Currencies & Exchange Rates", Subtitle = "Pricing configurations and conversion rates", Category = "System Settings", Icon = "💰", Route = "/settings/currencies", Keywords = "currencies rates exchange money usd eur egp pricing conversion dollar euro pound" },
            new SearchItemDto { Id = "nav-nationalities", Title = "Nationalities & Country Codes", Subtitle = "Guest nationality options for booking forms", Category = "System Settings", Icon = "🏳️", Route = "/settings/nationalities", Keywords = "nationalities countries country codes nationality guest origin passport" },

            // === ANALYTICS ===
            new SearchItemDto { Id = "nav-reports", Title = "Analytics & Reports", Subtitle = "Revenue dashboards, booking trends, and performance KPIs", Category = "Analytics", Icon = "📈", Route = "/reports", Keywords = "reports analytics charts revenue stats dashboard metrics performance kpi trends graphs data export" },
            new SearchItemDto { Id = "nav-dashboard", Title = "Dashboard Overview", Subtitle = "System health, quick stats, and activity feed", Category = "Analytics", Icon = "📊", Route = "/dashboard", Keywords = "dashboard home overview summary stats activity feed welcome main" },

            // === COMMON SHORTCUT TERMS ===
            new SearchItemDto { Id = "nav-logout", Title = "Logout / Sign Out", Subtitle = "End your admin session securely", Category = "Account", Icon = "🚪", Route = "/logout", Keywords = "logout sign out exit end session signout log out" },
            new SearchItemDto { Id = "nav-email-settings", Title = "Email & Notification Settings", Subtitle = "SMTP configuration, booking email templates", Category = "System Settings", Icon = "📧", Route = "/profile", Keywords = "email smtp notifications templates booking confirmation email settings notification preferences" }
        };
    }
}

public class GlobalSearchResultDto
{
    public List<SearchItemDto> QuickActions { get; set; } = new();
    public List<SearchItemDto> Tours { get; set; } = new();
    public List<SearchItemDto> Destinations { get; set; } = new();
    public List<SearchItemDto> Categories { get; set; } = new();
    public List<SearchItemDto> TourTypes { get; set; } = new();
    public List<SearchItemDto> Bookings { get; set; } = new();
    public List<SearchItemDto> Inquiries { get; set; } = new();
}

public class SearchItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = "✦";
    public string? Badge { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}
