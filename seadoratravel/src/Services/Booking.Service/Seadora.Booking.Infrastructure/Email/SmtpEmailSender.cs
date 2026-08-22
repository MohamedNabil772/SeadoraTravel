using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Infrastructure.Configuration;

namespace Seadora.Booking.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, htmlBody, cancellationToken);
    }

    public async Task SendTemplatedAsync(string to, string subject, string templateHtml, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, templateHtml, cancellationToken);
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken cancellationToken = default)
    {
        await SendEmailInternalAsync(to, subject, htmlMessage, cancellationToken);
    }

    private async Task SendEmailInternalAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            _logger.LogInformation("Sending email to {To} with subject {Subject}", to, subject);
            
            // Note: System.Net.Mail.SmtpClient.SendMailAsync does not natively take a CancellationToken in older .NET 
            // but in modern .NET it does. We will use the overload that accepts it if available, or just the default.
            await client.SendMailAsync(mailMessage, cancellationToken);
            
            _logger.LogInformation("Successfully sent email to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}. Subject: {Subject}", to, subject);
            // Exception is caught and logged to prevent email failures from aborting transactions
        }
    }
}
