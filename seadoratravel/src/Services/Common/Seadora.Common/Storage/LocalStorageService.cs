using Seadora.Common.Storage;

namespace Seadora.Common.Storage;

public class LocalStorageService : IStorageService
{
    private readonly string _storagePath;

    public LocalStorageService(string storagePath)
    {
        _storagePath = storagePath;
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }

        var fileId = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        var filePath = Path.Combine(_storagePath, fileId);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        return fileId;
    }

    public Task<Stream> GetFileAsync(string fileId)
    {
        var filePath = Path.Combine(_storagePath, fileId);
        if (!File.Exists(filePath)) throw new FileNotFoundException();

        return Task.FromResult<Stream>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }

    public Task DeleteFileAsync(string fileId)
    {
        var filePath = Path.Combine(_storagePath, fileId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        return Task.CompletedTask;
    }
}
