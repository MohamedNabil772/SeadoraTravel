using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Seadora.Content.API.Controllers.Admin;

[ApiController]
[Route("api/admin/excel")]
[Route("api/excel")]
public class ExcelImportExportController : ControllerBase
{
    private readonly IContentDbContext _context;

    public ExcelImportExportController(IContentDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. DOWNLOAD TEMPLATES
    // ==========================================
    [HttpGet("template/{entity}")]
    public IActionResult DownloadTemplate(string entity)
    {
        var entityType = entity?.Trim().ToLowerInvariant();
        using var workbook = new XLWorkbook();

        switch (entityType)
        {
            case "tours":
            case "tour":
                BuildToursTemplate(workbook);
                break;
            case "destinations":
            case "destination":
                BuildDestinationsTemplate(workbook);
                break;
            case "categories":
            case "category":
                BuildCategoriesTemplate(workbook);
                break;
            default:
                return BadRequest($"Unknown entity type '{entity}'. Supported types: tours, destinations, categories.");
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Seadora_{entity}_Template.xlsx");
    }

    // ==========================================
    // 2. EXPORT DATA TO EXCEL
    // ==========================================
    [HttpGet("export/{entity}")]
    public async Task<IActionResult> ExportData(string entity)
    {
        var entityType = entity?.Trim().ToLowerInvariant();
        using var workbook = new XLWorkbook();

        switch (entityType)
        {
            case "tours":
            case "tour":
                await ExportTours(workbook);
                break;
            case "destinations":
            case "destination":
                await ExportDestinations(workbook);
                break;
            case "categories":
            case "category":
                await ExportCategories(workbook);
                break;
            default:
                return BadRequest($"Unknown entity type '{entity}'. Supported types: tours, destinations, categories.");
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Seadora_{entity}_Export_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    // ==========================================
    // 3. IMPORT DATA FROM EXCEL
    // ==========================================
    [HttpPost("import/{entity}")]
    public async Task<IActionResult> ImportData(string entity, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Invalid file type. Please upload a .xlsx Excel workbook." });

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var entityType = entity?.Trim().ToLowerInvariant();

        switch (entityType)
        {
            case "tours":
            case "tour":
                return Ok(await ImportTours(workbook));
            case "destinations":
            case "destination":
                return Ok(await ImportDestinations(workbook));
            case "categories":
            case "category":
                return Ok(await ImportCategories(workbook));
            default:
                return BadRequest(new { message = $"Unknown entity type '{entity}'." });
        }
    }

    // ==========================================
    // PRIVATE TEMPLATE BUILDERS
    // ==========================================
    private static void StyleHeader(IXLRow row)
    {
        row.Style.Font.Bold = true;
        row.Style.Font.FontColor = XLColor.White;
        row.Style.Fill.BackgroundColor = XLColor.FromHtml("#0B1B3D"); // Seadora Navy
        row.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }

    private static void BuildToursTemplate(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Tours");
        var headers = new[]
        {
            "Title (EN)*", "Title (DE)", "Title (RU)", "Title (IT)", "Title (FR)",
            "Price*", "Currency*", "Duration", "Emoji", "Badge",
            "Category Name*", "Destination Name*", "Trip Type Code (GROUP/PRIVATE/VIP/YACHT)",
            "Description (EN)", "Description (DE)"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        // Sample Data Row
        ws.Cell(2, 1).Value = "Orange Bay VIP Luxury Yacht Cruise";
        ws.Cell(2, 2).Value = "Orange Bay VIP Luxus-Yachtfahrt";
        ws.Cell(2, 3).Value = "VIP Круиз на Остров Оранж Бэй";
        ws.Cell(2, 4).Value = "Crociera VIP a Orange Bay";
        ws.Cell(2, 5).Value = "Croisière VIP à Orange Bay";
        ws.Cell(2, 6).Value = 65.00;
        ws.Cell(2, 7).Value = "EUR";
        ws.Cell(2, 8).Value = "Full Day";
        ws.Cell(2, 9).Value = "🛥️";
        ws.Cell(2, 10).Value = "VIP Bestseller";
        ws.Cell(2, 11).Value = "Boat & Sea Trips";
        ws.Cell(2, 12).Value = "Hurghada";
        ws.Cell(2, 13).Value = "YACHT";
        ws.Cell(2, 14).Value = "Exclusive private luxury boat cruise with fresh seafood lunch.";
        ws.Cell(2, 15).Value = "Exklusive private Luxus-Bootstour mit Meeresfrüchte-Mittagessen.";

        ws.Columns().AdjustToContents();
    }

    private static void BuildDestinationsTemplate(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Destinations");
        var headers = new[]
        {
            "Name (EN)*", "Name (DE)", "Name (RU)", "Name (IT)", "Name (FR)",
            "Flag Emoji", "Description (EN)", "Description (DE)", "Highlights (EN)"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        ws.Cell(2, 1).Value = "El Gouna";
        ws.Cell(2, 2).Value = "El Gouna";
        ws.Cell(2, 3).Value = "Эль Гуна";
        ws.Cell(2, 4).Value = "El Gouna";
        ws.Cell(2, 5).Value = "El Gouna";
        ws.Cell(2, 6).Value = "🏖️";
        ws.Cell(2, 7).Value = "The Venice of the Red Sea with turquoise lagoons and luxury marinas.";
        ws.Cell(2, 8).Value = "Das Venedig des Roten Meeres mit türkisfarbenen Lagunen.";
        ws.Cell(2, 9).Value = "Lagoon Cruises, Marina Dining, Kitesurfing";

        ws.Columns().AdjustToContents();
    }

    private static void BuildCategoriesTemplate(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Categories");
        var headers = new[]
        {
            "Name (EN)*", "Name (DE)", "Name (RU)", "Name (IT)", "Name (FR)",
            "Icon (Emoji/Name)", "Display Order", "Description (EN)", "Description (DE)"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        ws.Cell(2, 1).Value = "Helicopter & Aviation";
        ws.Cell(2, 2).Value = "Hubschrauber & Luftfahrt";
        ws.Cell(2, 3).Value = "Вертолеты и Авиация";
        ws.Cell(2, 4).Value = "Elicottero e Aviazione";
        ws.Cell(2, 5).Value = "Hélicoptère et Aviation";
        ws.Cell(2, 6).Value = "🚁";
        ws.Cell(2, 7).Value = 5;
        ws.Cell(2, 8).Value = "Scenic aerial flights and luxury helicopter transfers.";
        ws.Cell(2, 9).Value = "Malerische Rundflüge und Luxus-Helikoptertransfers.";

        ws.Columns().AdjustToContents();
    }

    // ==========================================
    // EXPORT IMPLEMENTATIONS
    // ==========================================
    private async Task ExportTours(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Tours");
        var headers = new[]
        {
            "ID", "Title (EN)", "Title (DE)", "Title (RU)", "Title (IT)", "Title (FR)",
            "Price", "Currency", "Duration", "Emoji", "Badge",
            "Category", "Destination", "Trip Type", "Rating", "Review Count"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        var tours = await _context.Tours
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Destination)
            .Include(t => t.TourType)
            .ToListAsync();

        int row = 2;
        foreach (var t in tours)
        {
            ws.Cell(row, 1).Value = t.Id.ToString();
            ws.Cell(row, 2).Value = t.Names.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 3).Value = t.Names.GetValueOrDefault("de") ?? "";
            ws.Cell(row, 4).Value = t.Names.GetValueOrDefault("ru") ?? "";
            ws.Cell(row, 5).Value = t.Names.GetValueOrDefault("it") ?? "";
            ws.Cell(row, 6).Value = t.Names.GetValueOrDefault("fr") ?? "";
            ws.Cell(row, 7).Value = (double)t.Price;
            ws.Cell(row, 8).Value = t.Currency;
            ws.Cell(row, 9).Value = t.Duration;
            ws.Cell(row, 10).Value = t.Emoji;
            ws.Cell(row, 11).Value = t.Badge;
            ws.Cell(row, 12).Value = t.Category?.Names.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 13).Value = t.Destination?.Names.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 14).Value = t.TourType?.Code ?? "";
            ws.Cell(row, 15).Value = (double)t.Rating;
            ws.Cell(row, 16).Value = t.ReviewCount;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private async Task ExportDestinations(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Destinations");
        var headers = new[]
        {
            "ID", "Name (EN)", "Name (DE)", "Name (RU)", "Name (IT)", "Name (FR)",
            "Flag Emoji", "Description (EN)", "Tours Count"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        var destinations = await _context.Destinations
            .AsNoTracking()
            .Include(d => d.Tours)
            .ToListAsync();

        int row = 2;
        foreach (var d in destinations)
        {
            ws.Cell(row, 1).Value = d.Id.ToString();
            ws.Cell(row, 2).Value = d.Names.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 3).Value = d.Names.GetValueOrDefault("de") ?? "";
            ws.Cell(row, 4).Value = d.Names.GetValueOrDefault("ru") ?? "";
            ws.Cell(row, 5).Value = d.Names.GetValueOrDefault("it") ?? "";
            ws.Cell(row, 6).Value = d.Names.GetValueOrDefault("fr") ?? "";
            ws.Cell(row, 7).Value = d.FlagEmoji;
            ws.Cell(row, 8).Value = d.Descriptions.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 9).Value = d.Tours?.Count ?? 0;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private async Task ExportCategories(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Categories");
        var headers = new[]
        {
            "ID", "Name (EN)", "Name (DE)", "Name (RU)", "Name (IT)", "Name (FR)",
            "Icon", "Display Order", "Description (EN)", "Tours Count"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }
        StyleHeader(ws.Row(1));

        var categories = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Tours)
            .ToListAsync();

        int row = 2;
        foreach (var c in categories)
        {
            ws.Cell(row, 1).Value = c.Id.ToString();
            ws.Cell(row, 2).Value = c.Names.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 3).Value = c.Names.GetValueOrDefault("de") ?? "";
            ws.Cell(row, 4).Value = c.Names.GetValueOrDefault("ru") ?? "";
            ws.Cell(row, 5).Value = c.Names.GetValueOrDefault("it") ?? "";
            ws.Cell(row, 6).Value = c.Names.GetValueOrDefault("fr") ?? "";
            ws.Cell(row, 7).Value = c.IconName ?? "";
            ws.Cell(row, 8).Value = c.Order;
            ws.Cell(row, 9).Value = c.Descriptions.GetValueOrDefault("en") ?? "";
            ws.Cell(row, 10).Value = c.Tours?.Count ?? 0;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    // ==========================================
    // IMPORT IMPLEMENTATIONS
    // ==========================================
    private async Task<object> ImportTours(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.FirstOrDefault();
        if (ws == null) return new { success = false, message = "Empty workbook." };

        int rowCount = ws.LastRowUsed()?.RowNumber() ?? 1;
        int imported = 0;
        int updated = 0;
        var errors = new List<string>();

        var categories = await _context.Categories.ToListAsync();
        var destinations = await _context.Destinations.ToListAsync();
        var tourTypes = await _context.TourTypes.ToListAsync();

        for (int r = 2; r <= rowCount; r++)
        {
            var row = ws.Row(r);
            var titleEn = row.Cell(1).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(titleEn)) continue;

            try
            {
                var priceStr = row.Cell(6).GetString();
                decimal.TryParse(priceStr, out decimal price);
                var currency = row.Cell(7).GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(currency)) currency = "EUR";

                var duration = row.Cell(8).GetString()?.Trim() ?? "Full Day";
                var emoji = row.Cell(9).GetString()?.Trim() ?? "⛵";
                var badge = row.Cell(10).GetString()?.Trim() ?? "";
                var catName = row.Cell(11).GetString()?.Trim();
                var destName = row.Cell(12).GetString()?.Trim();
                var typeCode = row.Cell(13).GetString()?.Trim();

                var category = categories.FirstOrDefault(c => c.Names.Values.Any(v => v.Equals(catName, StringComparison.OrdinalIgnoreCase)))
                               ?? categories.FirstOrDefault();
                var destination = destinations.FirstOrDefault(d => d.Names.Values.Any(v => v.Equals(destName, StringComparison.OrdinalIgnoreCase)))
                                  ?? destinations.FirstOrDefault();
                var tourType = tourTypes.FirstOrDefault(tt => tt.Code.Equals(typeCode, StringComparison.OrdinalIgnoreCase));

                if (category == null || destination == null)
                {
                    errors.Add($"Row {r}: Category or Destination not found.");
                    continue;
                }

                // Check if tour with matching title exists
                var existing = await _context.Tours.FirstOrDefaultAsync(t => 
                    t.Names != null && t.Names.ContainsKey("en") && t.Names["en"].ToLower() == titleEn.ToLower());

                if (existing != null)
                {
                    existing.Price = price;
                    existing.Currency = currency;
                    existing.Duration = duration;
                    existing.Emoji = emoji;
                    existing.Badge = badge;
                    existing.CategoryId = category.Id;
                    existing.DestinationId = destination.Id;
                    if (tourType != null) existing.TourTypeId = tourType.Id;
                    updated++;
                }
                else
                {
                    var newTour = new Tour
                    {
                        Id = Guid.NewGuid(),
                        Price = price,
                        Currency = currency,
                        Duration = duration,
                        Emoji = emoji,
                        Badge = badge,
                        CategoryId = category.Id,
                        DestinationId = destination.Id,
                        TourTypeId = tourType?.Id,
                        Names = new Dictionary<string, string>
                        {
                            { "en", titleEn },
                            { "de", row.Cell(2).GetString()?.Trim() ?? titleEn },
                            { "ru", row.Cell(3).GetString()?.Trim() ?? titleEn },
                            { "it", row.Cell(4).GetString()?.Trim() ?? titleEn },
                            { "fr", row.Cell(5).GetString()?.Trim() ?? titleEn }
                        },
                        Descriptions = new Dictionary<string, string>
                        {
                            { "en", row.Cell(14).GetString()?.Trim() ?? titleEn },
                            { "de", row.Cell(15).GetString()?.Trim() ?? "" }
                        }
                    };
                    _context.Tours.Add(newTour);
                    imported++;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Row {r}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync(default);
        return new { success = true, totalRows = rowCount - 1, imported, updated, errors };
    }

    private async Task<object> ImportDestinations(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.FirstOrDefault();
        if (ws == null) return new { success = false, message = "Empty workbook." };

        int rowCount = ws.LastRowUsed()?.RowNumber() ?? 1;
        int imported = 0;
        int updated = 0;
        var errors = new List<string>();

        for (int r = 2; r <= rowCount; r++)
        {
            var row = ws.Row(r);
            var nameEn = row.Cell(1).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(nameEn)) continue;

            try
            {
                var flag = row.Cell(6).GetString()?.Trim() ?? "🗺️";
                var descEn = row.Cell(7).GetString()?.Trim() ?? "";

                var existing = await _context.Destinations.FirstOrDefaultAsync(d =>
                    d.Names != null && d.Names.ContainsKey("en") && d.Names["en"].ToLower() == nameEn.ToLower());

                if (existing != null)
                {
                    existing.FlagEmoji = flag;
                    existing.Descriptions["en"] = descEn;
                    updated++;
                }
                else
                {
                    var dest = new Destination
                    {
                        Id = Guid.NewGuid(),
                        FlagEmoji = flag,
                        Names = new Dictionary<string, string>
                        {
                            { "en", nameEn },
                            { "de", row.Cell(2).GetString()?.Trim() ?? nameEn },
                            { "ru", row.Cell(3).GetString()?.Trim() ?? nameEn },
                            { "it", row.Cell(4).GetString()?.Trim() ?? nameEn },
                            { "fr", row.Cell(5).GetString()?.Trim() ?? nameEn }
                        },
                        Descriptions = new Dictionary<string, string>
                        {
                            { "en", descEn },
                            { "de", row.Cell(8).GetString()?.Trim() ?? "" }
                        }
                    };
                    _context.Destinations.Add(dest);
                    imported++;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Row {r}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync(default);
        return new { success = true, totalRows = rowCount - 1, imported, updated, errors };
    }

    private async Task<object> ImportCategories(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.FirstOrDefault();
        if (ws == null) return new { success = false, message = "Empty workbook." };

        int rowCount = ws.LastRowUsed()?.RowNumber() ?? 1;
        int imported = 0;
        int updated = 0;
        var errors = new List<string>();

        for (int r = 2; r <= rowCount; r++)
        {
            var row = ws.Row(r);
            var nameEn = row.Cell(1).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(nameEn)) continue;

            try
            {
                var icon = row.Cell(6).GetString()?.Trim() ?? "🏷️";
                int.TryParse(row.Cell(7).GetString(), out int order);
                var descEn = row.Cell(8).GetString()?.Trim() ?? "";

                var existing = await _context.Categories.FirstOrDefaultAsync(c =>
                    c.Names != null && c.Names.ContainsKey("en") && c.Names["en"].ToLower() == nameEn.ToLower());

                if (existing != null)
                {
                    existing.IconName = icon;
                    existing.Order = order;
                    existing.Descriptions["en"] = descEn;
                    updated++;
                }
                else
                {
                    var cat = new Category
                    {
                        Id = Guid.NewGuid(),
                        IconName = icon,
                        Order = order,
                        Names = new Dictionary<string, string>
                        {
                            { "en", nameEn },
                            { "de", row.Cell(2).GetString()?.Trim() ?? nameEn },
                            { "ru", row.Cell(3).GetString()?.Trim() ?? nameEn },
                            { "it", row.Cell(4).GetString()?.Trim() ?? nameEn },
                            { "fr", row.Cell(5).GetString()?.Trim() ?? nameEn }
                        },
                        Descriptions = new Dictionary<string, string>
                        {
                            { "en", descEn },
                            { "de", row.Cell(9).GetString()?.Trim() ?? "" }
                        }
                    };
                    _context.Categories.Add(cat);
                    imported++;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Row {r}: {ex.Message}");
            }
        }

        await _context.SaveChangesAsync(default);
        return new { success = true, totalRows = rowCount - 1, imported, updated, errors };
    }
}
