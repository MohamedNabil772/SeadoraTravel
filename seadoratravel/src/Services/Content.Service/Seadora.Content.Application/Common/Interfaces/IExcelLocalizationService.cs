namespace Seadora.Content.Application.Common.Interfaces;

public interface IExcelLocalizationService
{
    Task<byte[]> GenerateTemplateWorkbookAsync(CancellationToken cancellationToken = default);
    Task ImportTranslationsAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
