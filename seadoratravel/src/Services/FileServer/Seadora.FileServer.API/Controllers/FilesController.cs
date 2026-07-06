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

        using var stream = file.OpenReadStream();
        var fileId = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);

        return Ok(new { FileId = fileId });
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> Download(string fileId)
    {
        try
        {
            var stream = await _storageService.GetFileAsync(fileId);
            return File(stream, "application/octet-stream", fileId);
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
}
