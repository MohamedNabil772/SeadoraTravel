using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Seadora.Identity.Application.Authentication.Commands.SendWhatsAppOtp;

public record SendWhatsAppOtpCommand(string PhoneNumber) : IRequest<SendWhatsAppOtpResponse>;

public record SendWhatsAppOtpResponse(bool Success, string Message);

public class SendWhatsAppOtpCommandHandler : IRequestHandler<SendWhatsAppOtpCommand, SendWhatsAppOtpResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<SendWhatsAppOtpCommandHandler> _logger;

    public SendWhatsAppOtpCommandHandler(IMemoryCache cache, ILogger<SendWhatsAppOtpCommandHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<SendWhatsAppOtpResponse> Handle(SendWhatsAppOtpCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate 6-digit OTP
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();

        // 2. Cache it for 5 mins
        var cacheKey = $"OTP_{request.PhoneNumber}";
        _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

        // 3. Format message
        var message = $@"🔐 *SeeDora Travel Verification Code*

Your security code is: *{otp}*

Valid for 5 minutes. Please do not share this code with anyone.

✨ Luxury Egypt Journeys · Concierge: +201068940967";

        // 4. Send via Twilio (mocked for now, fallback to Console logging)
        Console.WriteLine($"[TWILIO MOCK OTP: {request.PhoneNumber} -> {otp}]");
        _logger.LogInformation("OTP sent to {Phone}. Message: {Message}", request.PhoneNumber, message);

        return new SendWhatsAppOtpResponse(true, "OTP sent via WhatsApp");
    }
}
