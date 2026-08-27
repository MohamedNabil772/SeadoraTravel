namespace Seadora.Customer.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string FullName { get; set; } = string.Empty;
    // stored normalized (trim + lowercase) - the unique index is on (BranchId, Email)
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Nationality { get; set; }
    public string? PassportNumber { get; set; }
    public string? Notes { get; set; }
    public bool MarketingConsent { get; set; }
    public DateTime? ConsentUpdatedUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public List<CustomerDocument> Documents { get; set; } = new();

    public static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
