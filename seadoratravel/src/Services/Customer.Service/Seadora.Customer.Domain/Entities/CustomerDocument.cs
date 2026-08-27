namespace Seadora.Customer.Domain.Entities;

public class CustomerDocument
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    // ponytail: opaque handle only - the bytes and their encryption-at-rest are owned by FileServer.
    public string FileRef { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedUtc { get; set; }
    public DateTime? RetentionUntilUtc { get; set; }
}
