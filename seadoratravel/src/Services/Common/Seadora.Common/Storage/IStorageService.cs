namespace Seadora.Common.Storage;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> GetFileAsync(string fileId);
    Task DeleteFileAsync(string fileId);
}
