using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Seadora.Content.Application.Common.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Seadora.Content.API.Controllers.Admin;

[ApiController]
[Route("api/admin/pdf")]
[Route("api/pdf")]
public class PdfCatalogController : ControllerBase
{
    private readonly IContentDbContext _context;

    public PdfCatalogController(IContentDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateCatalogPdf([FromQuery] string language = "en")
    {
        var tours = await _context.Tours
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Destination)
            .Include(t => t.TourType)
            .ToListAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("SEADORA LUXURY TRAVEL").FontSize(20).Bold().FontColor("#0B1B3D");
                        col.Item().Text("VIP Experience Catalog & Master Portfolio").FontSize(10).Italic().FontColor("#D4AF37");
                    });

                    row.ConstantItem(120).AlignRight().Text($"Date: {DateTime.UtcNow:dd MMM yyyy}").FontSize(8).FontColor(Colors.Grey.Medium);
                });

                // Content
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().PaddingBottom(10).LineHorizontal(1).LineColor("#D4AF37");

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.5f);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background("#0B1B3D").Padding(5).Text("#").FontColor(Colors.White).Bold();
                            header.Cell().Background("#0B1B3D").Padding(5).Text("Tour Title").FontColor(Colors.White).Bold();
                            header.Cell().Background("#0B1B3D").Padding(5).Text("Destination").FontColor(Colors.White).Bold();
                            header.Cell().Background("#0B1B3D").Padding(5).Text("Category").FontColor(Colors.White).Bold();
                            header.Cell().Background("#0B1B3D").Padding(5).Text("Trip Type").FontColor(Colors.White).Bold();
                            header.Cell().Background("#0B1B3D").Padding(5).Text("Price").FontColor(Colors.White).Bold();
                        });

                        int idx = 1;
                        foreach (var tour in tours)
                        {
                            var bgColor = idx % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                            var title = tour.Names.GetValueOrDefault(language) ?? tour.Names.GetValueOrDefault("en") ?? "Untitled Tour";
                            var dest = tour.Destination?.Names.GetValueOrDefault(language) ?? tour.Destination?.Names.GetValueOrDefault("en") ?? "Global";
                            var cat = tour.Category?.Names.GetValueOrDefault(language) ?? tour.Category?.Names.GetValueOrDefault("en") ?? "General";
                            var type = tour.TourType?.Code ?? "Standard";

                            table.Cell().Background(bgColor).Padding(5).Text(idx.ToString());
                            table.Cell().Background(bgColor).Padding(5).Text(title).Bold();
                            table.Cell().Background(bgColor).Padding(5).Text(dest);
                            table.Cell().Background(bgColor).Padding(5).Text(cat);
                            table.Cell().Background(bgColor).Padding(5).Text(type);
                            table.Cell().Background(bgColor).Padding(5).Text($"{tour.Price:N2} {tour.Currency}").Bold().FontColor("#0B1B3D");
                            idx++;
                        }
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Seadora Travel • Luxury VIP Concierge • Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return File(stream.ToArray(), "application/pdf", $"Seadora_Catalog_{DateTime.UtcNow:yyyyMMdd}.pdf");
    }
}
