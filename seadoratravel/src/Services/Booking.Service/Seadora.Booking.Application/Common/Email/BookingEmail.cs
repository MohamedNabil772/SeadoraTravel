using System;
using System.Net;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Application.Common.Email;

public static class BookingEmail
{
    // ponytail: static policy copy mirroring the drafted tiers in
    // Domain/Services/CancellationPolicyService. Wire that service in here once its
    // refund rules are officially activated.
    public const string CancellationPolicyHtml =
        "<ul style=\"margin:8px 0 0;padding-left:20px;\">" +
        "<li><strong>More than 72 hours</strong> before departure: free cancellation, full refund.</li>" +
        "<li><strong>48\u201372 hours</strong> before departure: 25% cancellation charge.</li>" +
        "<li><strong>Less than 24 hours</strong> or no-show: 50% cancellation charge.</li>" +
        "</ul>";

    public static string BuildConfirmationHtml(Domain.Entities.Booking booking)
    {
        var name = WebUtility.HtmlEncode(booking.CustomerName);
        var reference = WebUtility.HtmlEncode(booking.Id.ToString());
        var date = booking.BookingDate.ToString("dddd, dd MMMM yyyy");

        return
            $"<p>Dear {name},</p>" +
            "<p>Great news \u2014 your booking with Seadora Travel is <strong>confirmed</strong>. " +
            "We can't wait to host you on the Red Sea.</p>" +
            "<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin:16px 0;font-size:15px;\">" +
            $"<tr><td style=\"padding:4px 16px 4px 0;color:#7a8b96;\">Booking reference</td><td style=\"padding:4px 0;font-weight:600;\">{reference}</td></tr>" +
            $"<tr><td style=\"padding:4px 16px 4px 0;color:#7a8b96;\">Booked on</td><td style=\"padding:4px 0;font-weight:600;\">{WebUtility.HtmlEncode(date)}</td></tr>" +
            "</table>" +
            "<h2 style=\"font-size:16px;color:#0a5c8a;margin:24px 0 4px;\">Cancellation policy</h2>" +
            CancellationPolicyHtml +
            "<p style=\"margin-top:24px;\">Questions? Just reply to this email or reach us at " +
            "<a href=\"mailto:info@seadoratravel.com\" style=\"color:#0a5c8a;\">info@seadoratravel.com</a>.</p>" +
            "<p>Warm regards,<br/>The Seadora Travel Team</p>";
    }
}
