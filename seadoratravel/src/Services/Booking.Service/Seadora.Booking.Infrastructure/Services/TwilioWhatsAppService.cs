using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seadora.Booking.Application.Common.Interfaces;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Infrastructure.Configuration;

namespace Seadora.Booking.Infrastructure.Services;

public class TwilioWhatsAppService : IWhatsAppNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly TwilioSettings _settings;
    private readonly ILogger<TwilioWhatsAppService> _logger;
    private const string TwilioApiUrl = "https://api.twilio.com/2010-04-01/Accounts/{0}/Messages.json";

    public TwilioWhatsAppService(HttpClient httpClient, IOptions<TwilioSettings> settings, ILogger<TwilioWhatsAppService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendBookingConfirmationAsync(Domain.Entities.Booking booking, CancellationToken cancellationToken = default)
    {
        // Mock handling and enabled check is delegated to SendCustomMessageAsync

        if (string.IsNullOrWhiteSpace(booking.WhatsApp))
        {
            _logger.LogWarning("Cannot send WhatsApp confirmation. No WhatsApp number provided for Booking ID: {BookingId}", booking.Id);
            return false;
        }

        string message = $@"🌟 *SeeDora Travel - Booking Confirmation* 🌟

Dear {booking.CustomerName},
Thank you for choosing SeeDora Travel! Your booking has been confirmed.

*Booking Reference:* {booking.Id.ToString().Substring(0, 8).ToUpper()}
*Tour Date:* {(booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dd MMM yyyy") : "TBD")}
*Pickup Time:* {(string.IsNullOrWhiteSpace(booking.PickupTime) ? "TBD" : booking.PickupTime)}
*Hotel:* {(string.IsNullOrWhiteSpace(booking.HotelName) ? "Not Specified" : booking.HotelName)}
*Room:* {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "N/A" : booking.RoomNumber)}
*Total Price:* ${booking.TotalPrice:F2}

Please be ready at the hotel lobby 10 minutes prior to your pickup time. 

If you need any assistance, contact our Concierge team:
*WhatsApp:* +201068940967

We wish you an unforgettable experience! 🌊🛥️";

        return await SendCustomMessageAsync(booking.WhatsApp, message, cancellationToken);
    }

    public async Task<bool> SendCustomMessageAsync(string toWhatsApp, string message, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.AccountSid))
        {
            _logger.LogInformation("[TWILIO MOCK WHATSAPP] To: {To} | Message:\n{Message}", toWhatsApp, message);
            return true;
        }

        if (string.IsNullOrWhiteSpace(toWhatsApp))
            return false;

        try
        {
            // Format phone number
            string formattedTo = FormatWhatsAppNumber(toWhatsApp);
            string formattedFrom = _settings.FromPhoneNumber.StartsWith("whatsapp:") 
                ? _settings.FromPhoneNumber 
                : $"whatsapp:{_settings.FromPhoneNumber}";

            var requestUrl = string.Format(TwilioApiUrl, _settings.AccountSid);

            var requestBody = new Dictionary<string, string>
            {
                { "To", formattedTo },
                { "From", formattedFrom },
                { "Body", message }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new FormUrlEncodedContent(requestBody)
            };

            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.AccountSid}:{_settings.AuthToken}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("WhatsApp message sent successfully to {To}", formattedTo);
                return true;
            }
            else
            {
                _logger.LogError("Failed to send WhatsApp message to {To}. Status: {Status}, Response: {Response}", formattedTo, response.StatusCode, responseContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending WhatsApp message to {ToWhatsApp}", toWhatsApp);
            return false;
        }
    }

    private string FormatWhatsAppNumber(string number)
    {
        var cleaned = number.Trim();
        if (!cleaned.StartsWith("+"))
        {
            cleaned = "+" + cleaned;
        }

        if (!cleaned.StartsWith("whatsapp:"))
        {
            cleaned = $"whatsapp:{cleaned}";
        }

        return cleaned;
    }
}
