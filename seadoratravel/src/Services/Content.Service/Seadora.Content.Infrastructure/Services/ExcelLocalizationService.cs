using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using System.Text.Json;

namespace Seadora.Content.Infrastructure.Services;

public class ExcelLocalizationService : IExcelLocalizationService
{
    private readonly IContentDbContext _context;
    private readonly string[] _supportedLocales = new[] { "en", "ar", "de", "fr", "it", "es", "ru" };

    public ExcelLocalizationService(IContentDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateTemplateWorkbookAsync(CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Translations");

        // Headers
        worksheet.Cell(1, 1).Value = "EntityType";
        worksheet.Cell(1, 2).Value = "EntityId";
        worksheet.Cell(1, 3).Value = "FieldName";

        for (int i = 0; i < _supportedLocales.Length; i++)
        {
            worksheet.Cell(1, 4 + i).Value = _supportedLocales[i];
        }

        int row = 2;

        // Categories
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        foreach (var category in categories)
        {
            AddTranslationRow(worksheet, ref row, "Category", category.Id.ToString(), "Names", category.Names);
            AddTranslationRow(worksheet, ref row, "Category", category.Id.ToString(), "Descriptions", category.Descriptions);
        }

        // Destinations
        var destinations = await _context.Destinations.ToListAsync(cancellationToken);
        foreach (var dest in destinations)
        {
            AddTranslationRow(worksheet, ref row, "Destination", dest.Id.ToString(), "Names", dest.Names);
            AddTranslationRow(worksheet, ref row, "Destination", dest.Id.ToString(), "Descriptions", dest.Descriptions);
            AddTranslationRow(worksheet, ref row, "Destination", dest.Id.ToString(), "Highlights", dest.Highlights);
        }

        // Tours
        var tours = await _context.Tours.ToListAsync(cancellationToken);
        foreach (var tour in tours)
        {
            AddTranslationRow(worksheet, ref row, "Tour", tour.Id.ToString(), "Names", tour.Names);
            AddTranslationRow(worksheet, ref row, "Tour", tour.Id.ToString(), "Descriptions", tour.Descriptions);
            AddTranslationRow(worksheet, ref row, "Tour", tour.Id.ToString(), "Highlights", tour.Highlights);
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void AddTranslationRow(IXLWorksheet worksheet, ref int row, string entityType, string entityId, string fieldName, Dictionary<string, string> values)
    {
        worksheet.Cell(row, 1).Value = entityType;
        worksheet.Cell(row, 2).Value = entityId;
        worksheet.Cell(row, 3).Value = fieldName;

        for (int i = 0; i < _supportedLocales.Length; i++)
        {
            var locale = _supportedLocales[i];
            if (values != null && values.TryGetValue(locale, out var val))
            {
                worksheet.Cell(row, 4 + i).Value = val;
            }
        }
        row++;
    }

    public async Task ImportTranslationsAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);

        var headerRow = worksheet.Row(1);
        var localeMap = new Dictionary<int, string>();

        for (int col = 4; col <= worksheet.LastCellUsed().Address.ColumnNumber; col++)
        {
            localeMap[col] = headerRow.Cell(col).GetString().Trim();
        }

        var categories = await _context.Categories.ToDictionaryAsync(c => c.Id, cancellationToken);
        var destinations = await _context.Destinations.ToDictionaryAsync(d => d.Id, cancellationToken);
        var tours = await _context.Tours.ToDictionaryAsync(t => t.Id, cancellationToken);

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var entityType = row.Cell(1).GetString().Trim();
            var entityIdStr = row.Cell(2).GetString().Trim();
            var fieldName = row.Cell(3).GetString().Trim();

            if (!Guid.TryParse(entityIdStr, out var entityId)) continue;

            Dictionary<string, string>? targetDict = null;

            if (entityType == "Category" && categories.TryGetValue(entityId, out var cat))
            {
                targetDict = fieldName switch
                {
                    "Names" => cat.Names ??= new(),
                    "Descriptions" => cat.Descriptions ??= new(),
                    _ => null
                };
            }
            else if (entityType == "Destination" && destinations.TryGetValue(entityId, out var dest))
            {
                targetDict = fieldName switch
                {
                    "Names" => dest.Names ??= new(),
                    "Descriptions" => dest.Descriptions ??= new(),
                    "Highlights" => dest.Highlights ??= new(),
                    _ => null
                };
            }
            else if (entityType == "Tour" && tours.TryGetValue(entityId, out var tour))
            {
                targetDict = fieldName switch
                {
                    "Names" => tour.Names ??= new(),
                    "Descriptions" => tour.Descriptions ??= new(),
                    "Highlights" => tour.Highlights ??= new(),
                    _ => null
                };
            }

            if (targetDict != null)
            {
                foreach (var kvp in localeMap)
                {
                    var col = kvp.Key;
                    var locale = kvp.Value;
                    var val = row.Cell(col).GetString();
                    
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        targetDict[locale] = val;
                    }
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
