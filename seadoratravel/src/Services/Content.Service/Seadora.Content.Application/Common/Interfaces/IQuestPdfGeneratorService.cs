namespace Seadora.Content.Application.Common.Interfaces;

public interface IQuestPdfGeneratorService
{
    Task<byte[]> GenerateTourBrochureAsync(Guid tourId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateTranslationAuditReportAsync(CancellationToken cancellationToken = default);
}
