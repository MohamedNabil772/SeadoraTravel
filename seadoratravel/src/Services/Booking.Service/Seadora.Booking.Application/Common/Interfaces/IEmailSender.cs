namespace Seadora.Booking.Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, string? replyTo = null, string? fromAddress = null, CancellationToken cancellationToken = default);

    // Wraps innerHtml in the branded Seadora template (logo/wordmark header + footer).
    Task SendTemplatedAsync(string to, string subject, string title, string innerHtml, string? replyTo = null, string? fromAddress = null, CancellationToken cancellationToken = default);
}
