namespace Seadora.Booking.Application.Common.Email;

// ponytail: brand identity is populated once at startup from the "Branding" config section
// (see BrandingOptions / ContactChannels.Configure). Defaults below are the current Seadora
// values so behavior is identical when no config is supplied. Static set-once is fine here:
// values are read-only after startup and shared by the (static) email builders.
public static class ContactChannels
{
    public static string InfoEmail { get; private set; } = "info@seadoratravel.com";
    public static string SupportEmail { get; private set; } = "support@seadoratravel.com";
    public static string DefaultSenderName { get; private set; } = "Seadora Travel";
    public static string WhatsAppNumber { get; private set; } = "+20 106 894 0967";
    public static string WhatsAppUrl { get; private set; } = "https://wa.me/201068940967";
    public static string WebsiteUrl { get; private set; } = "https://seadoratravel.com";
    public static string LogoUrl { get; private set; } = "https://seadoratravel.com/logo-emblem.png";
    public static string FeedbackBaseUrl { get; private set; } = "https://seadoratravel.com/feedback";

    public static void Configure(BrandingOptions? o)
    {
        if (o is null) return;
        if (!string.IsNullOrWhiteSpace(o.InfoEmail)) InfoEmail = o.InfoEmail;
        if (!string.IsNullOrWhiteSpace(o.SupportEmail)) SupportEmail = o.SupportEmail;
        if (!string.IsNullOrWhiteSpace(o.SenderName)) DefaultSenderName = o.SenderName;
        if (!string.IsNullOrWhiteSpace(o.WhatsAppNumber)) WhatsAppNumber = o.WhatsAppNumber;
        if (!string.IsNullOrWhiteSpace(o.WhatsAppUrl)) WhatsAppUrl = o.WhatsAppUrl;
        if (!string.IsNullOrWhiteSpace(o.WebsiteUrl)) WebsiteUrl = o.WebsiteUrl;
        if (!string.IsNullOrWhiteSpace(o.LogoUrl)) LogoUrl = o.LogoUrl;
        if (!string.IsNullOrWhiteSpace(o.FeedbackBaseUrl)) FeedbackBaseUrl = o.FeedbackBaseUrl;
    }
}
