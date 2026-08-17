using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.Infrastructure.Services;

public class QuestPdfGeneratorService : IQuestPdfGeneratorService
{
    private readonly IContentDbContext _context;

    public QuestPdfGeneratorService(IContentDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateTourBrochureAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        var tour = await _context.Tours
            .Include(t => t.Destination)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == tourId, cancellationToken);

        if (tour == null) return Array.Empty<byte>();

        var name = tour.Names.GetValueOrDefault("en", "Unnamed Tour");
        var description = tour.Descriptions.GetValueOrDefault("en", "No description available.");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Seadora Travel - {name}")
                    .SemiBold().FontSize(24).FontColor(Colors.Blue.Darken2);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text($"Destination: {tour.Destination?.Names.GetValueOrDefault("en", "Unknown")}").FontSize(14);
                        x.Item().Text($"Category: {tour.Category?.Names.GetValueOrDefault("en", "Unknown")}").FontSize(14);
                        x.Item().Text($"Price: {tour.Price} {tour.Currency}").FontSize(14);
                        x.Item().Text($"Duration: {tour.Duration}").FontSize(14);

                        x.Item().Text("Description:").SemiBold().FontSize(16);
                        x.Item().Text(description);

                        if (tour.Itinerary?.Any() == true)
                        {
                            x.Item().Text("Itinerary:").SemiBold().FontSize(16);
                            foreach (var item in tour.Itinerary)
                            {
                                x.Item().Text($"- {item.Title.GetValueOrDefault("en", "Day")}: {item.Description.GetValueOrDefault("en", "")}");
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateTranslationAuditReportAsync(CancellationToken cancellationToken = default)
    {
        var tours = await _context.Tours.ToListAsync(cancellationToken);
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);

                page.Header()
                    .Text("Translation Audit Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Red.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Entity Type").SemiBold();
                            header.Cell().Text("Entity ID").SemiBold();
                            header.Cell().Text("Missing EN").SemiBold();
                            header.Cell().Text("Missing AR").SemiBold();
                        });

                        foreach (var tour in tours)
                        {
                            bool missingEn = !tour.Names.ContainsKey("en");
                            bool missingAr = !tour.Names.ContainsKey("ar");

                            if (missingEn || missingAr)
                            {
                                table.Cell().Text("Tour");
                                table.Cell().Text(tour.Id.ToString());
                                table.Cell().Text(missingEn ? "Yes" : "No");
                                table.Cell().Text(missingAr ? "Yes" : "No");
                            }
                        }
                    });
            });
        });

        return document.GeneratePdf();
    }
}
