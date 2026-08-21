using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.Infrastructure.Email;

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromAddress { get; set; } = "noreply@seadoratravel.com";
    public string FromName { get; set; } = "Seadora Travel";
    // Optional hosted logo image URL. If empty, a text wordmark is used instead.
    public string LogoUrl { get; set; } = "";
}

// ponytail: System.Net.Mail relay to an external SMTP (Hostinger). Swap to MailKit
// only if you hit STARTTLS/OAuth edge cases SmtpClient can't handle.
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, string? replyTo = null, string? fromAddress = null, CancellationToken cancellationToken = default)
        => SendCoreAsync(to, subject, body, replyTo, fromAddress, cancellationToken);

    public Task SendTemplatedAsync(string to, string subject, string title, string innerHtml, string? replyTo = null, string? fromAddress = null, CancellationToken cancellationToken = default)
        => SendCoreAsync(to, subject, BuildBrandedHtml(title, innerHtml), replyTo, fromAddress, cancellationToken);

    private async Task SendCoreAsync(string to, string subject, string body, string? replyTo, string? fromAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            _logger.LogWarning("SMTP host not configured; skipping email to {To}.", to);
            return;
        }

        var from = string.IsNullOrWhiteSpace(fromAddress) ? _settings.FromAddress : fromAddress;

        using var message = new MailMessage
        {
            From = new MailAddress(from, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(to);
        if (!string.IsNullOrWhiteSpace(replyTo))
        {
            message.ReplyToList.Add(new MailAddress(replyTo));
        }

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(_settings.Username)
                ? null
                : new NetworkCredential(_settings.Username, _settings.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }

    // Branded, table-based shell (email-client safe). Logo image when configured,
    // otherwise a 🌊 wordmark header.
    private string BuildBrandedHtml(string title, string innerHtml)
    {
        var brand = WebUtility.HtmlEncode(_settings.FromName);
        var header = string.IsNullOrWhiteSpace(_settings.LogoUrl)
            ? $"<span style=\"font-size:26px;font-weight:700;color:#ffffff;font-family:Georgia,'Times New Roman',serif;\">\uD83C\uDF0A {brand}</span>"
            : $"<img src=\"{WebUtility.HtmlEncode(_settings.LogoUrl)}\" alt=\"{brand}\" height=\"48\" style=\"display:block;border:0;max-height:48px;\" />";

        return $@"<!DOCTYPE html>
<html>
<body style=""margin:0;padding:0;background:#eef4f7;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#eef4f7;padding:24px 0;"">
    <tr><td align=""center"">
      <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;width:100%;background:#ffffff;border-radius:12px;overflow:hidden;font-family:Arial,Helvetica,sans-serif;color:#243b4a;"">
        <tr><td style=""background:#0a5c8a;padding:24px 32px;"" align=""left"">{header}</td></tr>
        <tr><td style=""padding:32px;"">
          <h1 style=""margin:0 0 16px;font-size:22px;color:#0a5c8a;"">{WebUtility.HtmlEncode(title)}</h1>
          <div style=""font-size:15px;line-height:1.6;color:#3a4a55;"">{innerHtml}</div>
        </td></tr>
        <tr><td style=""background:#0a1929;padding:20px 32px;font-size:12px;color:#8eafc2;"" align=""center"">
          {brand} &middot; Hurghada, Red Sea, Egypt &middot;
          <a href=""mailto:info@seadoratravel.com"" style=""color:#f5a435;text-decoration:none;"">info@seadoratravel.com</a>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
    }
}
