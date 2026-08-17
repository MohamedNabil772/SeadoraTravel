using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/translations")]
public class TranslationsController : ControllerBase
{
    private readonly IExcelLocalizationService _excelLocalizationService;
    private readonly IQuestPdfGeneratorService _pdfService;

    public TranslationsController(IExcelLocalizationService excelLocalizationService, IQuestPdfGeneratorService pdfService)
    {
        _excelLocalizationService = excelLocalizationService;
        _pdfService = pdfService;
    }

    [HttpGet("template")]
    public async Task<IActionResult> DownloadTemplate(CancellationToken cancellationToken)
    {
        var fileBytes = await _excelLocalizationService.GenerateTemplateWorkbookAsync(cancellationToken);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "translations_template.xlsx");
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportTranslations(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty.");

        using var stream = file.OpenReadStream();
        await _excelLocalizationService.ImportTranslationsAsync(stream, cancellationToken);
        
        return Ok(new { message = "Translations imported successfully" });
    }

    [HttpGet("audit-report")]
    public async Task<IActionResult> DownloadAuditReport(CancellationToken cancellationToken)
    {
        var fileBytes = await _pdfService.GenerateTranslationAuditReportAsync(cancellationToken);
        return File(fileBytes, "application/pdf", "translation_audit_report.pdf");
    }
}
