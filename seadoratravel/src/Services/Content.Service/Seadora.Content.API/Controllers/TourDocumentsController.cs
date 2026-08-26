using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Common.Interfaces;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/tours/{tourId:guid}/documents")]
public class TourDocumentsController : ControllerBase
{
    private readonly IQuestPdfGeneratorService _pdfService;

    public TourDocumentsController(IQuestPdfGeneratorService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpGet("brochure")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadBrochure(Guid tourId, CancellationToken cancellationToken)
    {
        var fileBytes = await _pdfService.GenerateTourBrochureAsync(tourId, cancellationToken);
        
        if (fileBytes == null || fileBytes.Length == 0)
            return NotFound("Tour not found or brochure could not be generated.");
            
        return File(fileBytes, "application/pdf", $"tour_brochure_{tourId}.pdf");
    }
}
