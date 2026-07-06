using System.Net.Http.Headers;
using System.Net.Http.Json;
using Seadora.Common.Storage;

namespace Seadora.Common.Storage;

public class RemoteStorageService : IStorageService
{
    private readonly HttpClient _httpClient;

    public RemoteStorageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        var response = await _httpClient.PostAsync("api/files", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UploadResult>();
        return result?.FileId ?? throw new Exception("Upload failed");
    }

    public async Task<Stream> GetFileAsync(string fileId)
    {
        var response = await _httpClient.GetAsync($"api/files/{fileId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string fileId)
    {
        var response = await _httpClient.DeleteAsync($"api/files/{fileId}");
        response.EnsureSuccessStatusCode();
    }

    private class UploadResult
    {
        public string FileId { get; set; } = string.Empty;
    }
}
