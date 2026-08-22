namespace Seadora.Booking.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendTemplatedAsync(string to, string subject, string templateHtml, CancellationToken cancellationToken = default);
    Task SendEmailAsync(string to, string subject, string htmlMessage, CancellationToken cancellationToken = default);
}
