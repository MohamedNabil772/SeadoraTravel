using Microsoft.AspNetCore.Mvc;
using Seadora.Common.Storage;

namespace Seadora.FileServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IStorageService _storageService;

    public FilesController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".pdf", ".xlsx", ".xls", ".csv" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type.");

        if (file.Length > 15 * 1024 * 1024) // 15 MB
            return BadRequest("File size exceeds 15MB limit.");

        using var stream = file.OpenReadStream();
        var fileId = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

        return Ok(new { fileId = fileId, url = $"/api/files/{fileId}" });
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> Download(string fileId)
    {
        try
        {
            var stream = await _storageService.GetFileAsync(fileId);
            var contentType = GetContentType(fileId);
            return File(stream, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> Delete(string fileId)
    {
        await _storageService.DeleteFileAsync(fileId);
        return NoContent();
    }

    private string GetContentType(string path)
    {
        var types = new Dictionary<string, string>
        {
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".png", "image/png"},
            {".webp", "image/webp"},
            {".svg", "image/svg+xml"},
            {".pdf", "application/pdf"},
            {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {".xls", "application/vnd.ms-excel"},
            {".csv", "text/csv"}
        };

        var ext = Path.GetExtension(path).ToLowerInvariant();
        return types.TryGetValue(ext, out var contentType) ? contentType : "application/octet-stream";
    }
}
