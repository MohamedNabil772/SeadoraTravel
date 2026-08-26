namespace Seadora.Booking.Application.Common.Email;

// Per-deployment brand identity, bound from the "Branding" config section.
// Leave any value blank to keep the built-in default (current Seadora values).
public class BrandingOptions
{
    public const string SectionName = "Branding";

    public string? InfoEmail { get; set; }
    public string? SupportEmail { get; set; }
    public string? SenderName { get; set; }
    public string? WhatsAppNumber { get; set; }
    public string? WhatsAppUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? FeedbackBaseUrl { get; set; }
}
