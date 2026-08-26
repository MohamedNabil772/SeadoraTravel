using System;
using System.Text;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Application.Common.Email;

public static class BookingEmail
{
    private const string NavyDark = "#06152B";
    private const string GoldAccent = "#D4AF37";
    private const string GoldLight = "#F4D03F";
    private const string BackgroundCream = "#F7F5F0";
    private const string TextCharcoal = "#2A3F4F";
    private const string TextMuted = "#6B8A9A";
    private const string CardBorder = "#EAE3D6";
    private const string LogoUrl = "https://seadoratravel.com/logo-emblem.png";
    private const string WhatsAppUrl = "https://wa.me/201001296641";
    private const string WebsiteUrl = "https://seadoratravel.com";

    private static string GetEmailHeader(string preheader, string statusPill, string title, string subtitle)
    {
        return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>{title}</title>
            <style>
                @import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,600;0,700;1,400&family=Jost:wght@400;500;600;700&display=swap');
                body {{
                    margin: 0;
                    padding: 0;
                    background-color: {BackgroundCream};
                    font-family: 'Jost', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                    -webkit-font-smoothing: antialiased;
                    color: {TextCharcoal};
                }}
                table {{ border-collapse: separate; }}
                a {{ text-decoration: none; }}
                @media only screen and (max-width: 620px) {{
                    .email-container {{ width: 100% !important; padding: 10px !important; }}
                    .mobile-p-20 {{ padding: 24px 20px !important; }}
                    .mobile-stack {{ display: block !important; width: 100% !important; }}
                    .mobile-mb-10 {{ margin-bottom: 12px !important; }}
                }}
            </style>
        </head>
        <body style='margin: 0; padding: 0; background-color: {BackgroundCream};'>
            <!-- Preheader hidden preview text -->
            <div style='display: none; font-size: 1px; color: #fff; line-height: 1px; max-height: 0px; max-width: 0px; opacity: 0; overflow: hidden;'>
                {preheader}
            </div>

            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: {BackgroundCream}; width: 100%; padding: 40px 0;'>
                <tr>
                    <td align='center'>
                        <!-- Main Card -->
                        <table role='presentation' class='email-container' width='600' cellspacing='0' cellpadding='0' border='0' style='width: 600px; max-width: 600px; background-color: #ffffff; border-radius: 20px; overflow: hidden; border: 1px solid {CardBorder}; box-shadow: 0 12px 40px rgba(6,21,43,0.08);'>
                            
                            <!-- Header Banner -->
                            <tr>
                                <td style='background: linear-gradient(135deg, {NavyDark} 0%, #0D2342 100%); background-color: {NavyDark}; padding: 40px 32px; text-align: center; border-bottom: 3px solid {GoldAccent};'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <img src='{LogoUrl}' alt='Seadora Emblem' width='54' height='54' style='display: block; margin: 0 auto 16px auto; width: 54px; height: 54px; filter: drop-shadow(0 4px 12px rgba(212,175,55,0.4));' />
                                                <h1 style='margin: 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 24px; font-weight: 700; color: #FFFFFF; letter-spacing: 3px; text-transform: uppercase;'>
                                                    SEADORA TRAVEL
                                                </h1>
                                                <p style='margin: 4px 0 0 0; font-size: 11px; font-weight: 500; color: {GoldAccent}; letter-spacing: 2px; text-transform: uppercase;'>
                                                    Luxury Red Sea Concierge & Private Journeys
                                                </p>
                                                <div style='margin-top: 20px;'>
                                                    <span style='display: inline-block; padding: 6px 16px; background: rgba(212,175,55,0.15); border: 1px solid {GoldAccent}; border-radius: 30px; font-size: 11px; font-weight: 600; color: {GoldLight}; letter-spacing: 1.5px; text-transform: uppercase;'>
                                                        {statusPill}
                                                    </span>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>";
    }

    private static string GetEmailFooter()
    {
        return $@"
                            <!-- Concierge Contact Bar -->
                            <tr>
                                <td style='background-color: #FAF8F5; padding: 28px 36px; border-top: 1px solid {CardBorder}; text-align: center;'>
                                    <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                        <tr>
                                            <td align='center'>
                                                <p style='margin: 0 0 12px 0; font-size: 13px; font-weight: 600; color: {NavyDark}; text-transform: uppercase; letter-spacing: 1px;'>
                                                    Dedicated 24/7 VIP Concierge
                                                </p>
                                                <p style='margin: 0 0 18px 0; font-size: 13px; color: {TextMuted}; line-height: 1.5;'>
                                                    Our local Hurghada travel specialists are on standby to accommodate any special requests or bespoke itinerary adjustments.
                                                </p>
                                                <table role='presentation' cellspacing='0' cellpadding='0' border='0' align='center'>
                                                    <tr>
                                                        <td style='border-radius: 12px; background: linear-gradient(135deg, #25D366 0%, #128C7E 100%); background-color: #25D366; text-align: center; box-shadow: 0 6px 20px rgba(37,211,102,0.25);'>
                                                            <a href='{WhatsAppUrl}' target='_blank' style='display: inline-block; padding: 12px 26px; font-size: 13px; font-weight: 700; color: #ffffff; letter-spacing: 0.5px;'>
                                                                💬 Chat with Concierge on WhatsApp
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <!-- Dark Footer -->
                            <tr>
                                <td style='background-color: {NavyDark}; padding: 32px 24px; text-align: center; color: rgba(255,255,255,0.6); font-size: 12px; line-height: 1.6;'>
                                    <p style='margin: 0 0 8px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 14px; font-weight: 700; color: #FFFFFF; letter-spacing: 1.5px;'>
                                        SEADORA LUXURY TRAVEL
                                    </p>
                                    <p style='margin: 0 0 12px 0;'>
                                        Hurghada Marina, Red Sea Governorate, Egypt • Phone: +20 100 129 6641
                                    </p>
                                    <p style='margin: 0 0 16px 0; color: rgba(255,255,255,0.4); font-size: 11px;'>
                                        Direct Inquiries: <a href='mailto:{ContactChannels.InfoEmail}' style='color: {GoldAccent};'>{ContactChannels.InfoEmail}</a> • Website: <a href='{WebsiteUrl}' style='color: {GoldAccent};'>{WebsiteUrl}</a>
                                    </p>
                                    <div style='border-top: 1px solid rgba(255,255,255,0.1); padding-top: 16px; font-size: 11px; color: rgba(255,255,255,0.35);'>
                                        &copy; {DateTime.UtcNow.Year} Seadora Travel. All rights reserved. • System Architecture by TIM SOLUTIONS™
                                    </div>
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
    }

    public static string BuildReceiptHtml(Domain.Entities.Booking booking)
    {
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : "Date on Request";
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? "To be confirmed by Concierge" : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Hotel Pickup Requested" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            preheader: $"Your booking #{refCode} has been received and is being prepared with VIP care.",
            statusPill: "Reservation Request Received",
            title: "Booking Received — Seadora Travel",
            subtitle: "Your Egyptian Journey Begins"
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 36px 36px 28px 36px;'>
                <p style='margin: 0 0 8px 0; font-size: 13px; font-weight: 600; color: {GoldAccent}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    Warm Greetings
                </p>
                <h2 style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 22px; font-weight: 700; color: {NavyDark}; line-height: 1.3;'>
                    Dear {booking.CustomerName},
                </h2>
                <p style='margin: 0 0 24px 0; font-size: 14px; color: {TextCharcoal}; line-height: 1.7;'>
                    Thank you for selecting <strong>Seadora Luxury Travel</strong>. We have received your reservation request and our VIP operations team in Hurghada is currently reviewing your schedule to ensure every detail meets our five-star standards.
                </p>

                <!-- Ticket Card -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #FAF8F5; border: 1px solid {CardBorder}; border-radius: 16px; overflow: hidden; margin-bottom: 28px;'>
                    <tr>
                        <td style='background: {NavyDark}; padding: 14px 20px; border-bottom: 2px solid {GoldAccent};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <span style='font-size: 11px; font-weight: 600; color: rgba(255,255,255,0.6); text-transform: uppercase; letter-spacing: 1px;'>
                                            Voucher Reference
                                        </span>
                                        <div style='font-family: ""Playfair Display"", serif; font-size: 18px; font-weight: 700; color: {GoldLight}; letter-spacing: 2px;'>
                                            #{refCode}
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <span style='display: inline-block; padding: 4px 10px; background: rgba(255,255,255,0.1); border-radius: 6px; font-size: 11px; font-weight: 600; color: #ffffff;'>
                                            {(booking.TripType ?? "Private Experience")}
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 20px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td width='50%' valign='top' style='padding-bottom: 16px;'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Experience Date</div>
                                        <div style='font-size: 14px; font-weight: 600; color: {NavyDark}; margin-top: 4px;'>{tourDateFormatted}</div>
                                    </td>
                                    <td width='50%' valign='top' style='padding-bottom: 16px;'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Pickup Window</div>
                                        <div style='font-size: 14px; font-weight: 600; color: {NavyDark}; margin-top: 4px;'>{pickupInfo}</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width='50%' valign='top'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Guests</div>
                                        <div style='font-size: 14px; font-weight: 600; color: {NavyDark}; margin-top: 4px;'>{booking.Guests} {(booking.Guests == 1 ? "Guest" : "Guests")}</div>
                                    </td>
                                    <td width='50%' valign='top'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Total Amount</div>
                                        <div style='font-size: 16px; font-weight: 700; color: {GoldAccent}; margin-top: 4px;'>${booking.TotalPrice:N2}</div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #F1ECE3; padding: 12px 20px; border-top: 1px dashed {CardBorder}; font-size: 12px; color: {TextCharcoal};'>
                            <strong>Pickup Location:</strong> {hotelInfo}
                        </td>
                    </tr>
                </table>

                <!-- Next Steps Info Box -->
                <div style='background-color: #FDFCFA; border-left: 4px solid {GoldAccent}; padding: 16px 20px; border-radius: 8px; margin-bottom: 24px;'>
                    <h4 style='margin: 0 0 6px 0; font-size: 13px; font-weight: 700; color: {NavyDark}; text-transform: uppercase; letter-spacing: 0.5px;'>
                        What Happens Next?
                    </h4>
                    <p style='margin: 0; font-size: 13px; color: {TextCharcoal}; line-height: 1.6;'>
                        Our concierge will verify timing with our private transport and cruise team. You will receive an official confirmation voucher and our WhatsApp team will reach out with your driver's direct details prior to pickup.
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter());
        return sb.ToString();
    }

    public static string BuildConfirmationHtml(Domain.Entities.Booking booking)
    {
        var refCode = booking.Id.ToString().Substring(0, 8).ToUpper();
        var tourDateFormatted = booking.TourDate.HasValue ? booking.TourDate.Value.ToString("dddd, MMMM dd, yyyy") : "Confirmed Date";
        var pickupInfo = string.IsNullOrWhiteSpace(booking.PickupTime) ? "09:00 AM (Sharp)" : booking.PickupTime;
        var hotelInfo = string.IsNullOrWhiteSpace(booking.HotelName) ? "Private Luxury Vehicle Transfer" : $"{booking.HotelName} {(string.IsNullOrWhiteSpace(booking.RoomNumber) ? "" : $"(Room {booking.RoomNumber})")}";

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            preheader: $"Your VIP Booking #{refCode} is CONFIRMED! View your official travel voucher inside.",
            statusPill: "Officially Confirmed",
            title: "Booking Confirmed — Seadora Travel",
            subtitle: "Your Private VIP Itinerary"
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 36px 36px 28px 36px;'>
                <p style='margin: 0 0 8px 0; font-size: 13px; font-weight: 600; color: #2E7D4F; text-transform: uppercase; letter-spacing: 1.5px;'>
                    ✓ VIP Voucher Activated
                </p>
                <h2 style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 22px; font-weight: 700; color: {NavyDark}; line-height: 1.3;'>
                    Dear {booking.CustomerName},
                </h2>
                <p style='margin: 0 0 24px 0; font-size: 14px; color: {TextCharcoal}; line-height: 1.7;'>
                    We are thrilled to confirm your luxury experience with <strong>Seadora Travel</strong>. Your private vehicle and licensed tour guide are locked in. Please keep this voucher accessible on your mobile device.
                </p>

                <!-- Boarding Pass Style Voucher Card -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #FAF8F5; border: 2px solid {GoldAccent}; border-radius: 16px; overflow: hidden; margin-bottom: 28px; box-shadow: 0 8px 24px rgba(212,175,55,0.12);'>
                    <tr>
                        <td style='background: {NavyDark}; padding: 18px 22px; border-bottom: 2px solid {GoldAccent};'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td>
                                        <span style='font-size: 10px; font-weight: 700; color: {GoldAccent}; text-transform: uppercase; letter-spacing: 1.5px;'>
                                            OFFICIAL TRAVEL VOUCHER
                                        </span>
                                        <div style='font-family: ""Playfair Display"", serif; font-size: 20px; font-weight: 700; color: #FFFFFF; letter-spacing: 2px;'>
                                            #{refCode}
                                        </div>
                                    </td>
                                    <td align='right'>
                                        <div style='display: inline-block; padding: 6px 14px; background: #2E7D4F; border-radius: 20px; font-size: 11px; font-weight: 700; color: #ffffff; letter-spacing: 1px; text-transform: uppercase;'>
                                            Confirmed
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 20px;'>
                            <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0'>
                                <tr>
                                    <td width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Tour Date</div>
                                        <div style='font-size: 15px; font-weight: 700; color: {NavyDark}; margin-top: 4px;'>{tourDateFormatted}</div>
                                    </td>
                                    <td width='50%' valign='top' style='padding-bottom: 18px;'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Pickup Window</div>
                                        <div style='font-size: 15px; font-weight: 700; color: {NavyDark}; margin-top: 4px;'>{pickupInfo}</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td width='50%' valign='top'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Party Size</div>
                                        <div style='font-size: 14px; font-weight: 600; color: {NavyDark}; margin-top: 4px;'>{booking.Guests} Guests</div>
                                    </td>
                                    <td width='50%' valign='top'>
                                        <div style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Total Amount</div>
                                        <div style='font-size: 18px; font-weight: 700; color: {GoldAccent}; margin-top: 4px;'>${booking.TotalPrice:N2}</div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color: #F3ECE1; padding: 14px 20px; border-top: 1px dashed {CardBorder}; font-size: 12px; color: {NavyDark};'>
                            📍 <strong>Meeting / Pickup Point:</strong> {hotelInfo}
                        </td>
                    </tr>
                </table>

                <!-- VIP Travel Guidelines Box -->
                <div style='background-color: #FAF7F2; border-left: 4px solid {NavyDark}; padding: 18px 20px; border-radius: 8px; margin-bottom: 24px;'>
                    <h4 style='margin: 0 0 8px 0; font-size: 13px; font-weight: 700; color: {NavyDark}; text-transform: uppercase; letter-spacing: 0.5px;'>
                        Important Reminders for Tour Day
                    </h4>
                    <ul style='margin: 0; padding-left: 18px; font-size: 13px; color: {TextCharcoal}; line-height: 1.6;'>
                        <li style='margin-bottom: 6px;'>Please carry a physical copy or mobile photo of your passport / national ID.</li>
                        <li style='margin-bottom: 6px;'>Be in the hotel lobby 10 minutes prior to the scheduled pickup time.</li>
                        <li>Sunscreen, sunglasses, and comfortable resort wear are strongly advised for desert & boat activities.</li>
                    </ul>
                </div>

                <!-- Cancellation Guarantee -->
                <div style='background-color: #FDF3E0; border: 1px solid #F5A435; padding: 14px 18px; border-radius: 8px; font-size: 12px; color: #8A4F00; line-height: 1.5;'>
                    <strong>Flexibility Policy:</strong> Free cancellation & full refund available up to 48 hours before departure. Reach our concierge anytime for date changes.
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter());
        return sb.ToString();
    }

    public static string BuildInquiryAutoReplyHtml(ContactInquiry inquiry)
    {
        var destination = string.IsNullOrWhiteSpace(inquiry.DestinationInterest) ? "Egypt & The Red Sea" : inquiry.DestinationInterest;

        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            preheader: $"Thank you for contacting Seadora Travel regarding {destination}.",
            statusPill: "Inquiry Received",
            title: "Thank You for Contacting Seadora Travel",
            subtitle: "Bespoke Travel Inquiry"
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 36px 36px 28px 36px;'>
                <p style='margin: 0 0 8px 0; font-size: 13px; font-weight: 600; color: {GoldAccent}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    Inquiry Acknowledgment
                </p>
                <h2 style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 22px; font-weight: 700; color: {NavyDark}; line-height: 1.3;'>
                    Dear {inquiry.FullName},
                </h2>
                <p style='margin: 0 0 20px 0; font-size: 14px; color: {TextCharcoal}; line-height: 1.7;'>
                    Thank you for reaching out to <strong>Seadora Luxury Travel</strong>. We have received your inquiry regarding <strong>{destination}</strong>.
                </p>

                <!-- Inquiry Summary Box -->
                <table role='presentation' width='100%' cellspacing='0' cellpadding='0' border='0' style='background-color: #FAF8F5; border: 1px solid {CardBorder}; border-radius: 12px; padding: 18px; margin-bottom: 24px;'>
                    <tr>
                        <td>
                            <div style='font-size: 11px; font-weight: 700; color: {NavyDark}; text-transform: uppercase; letter-spacing: 1px; margin-bottom: 8px;'>Your Message / Request:</div>
                            <div style='font-size: 13px; color: {TextCharcoal}; font-style: italic; line-height: 1.6;'>
                                ""{inquiry.Message}""
                            </div>
                            {(string.IsNullOrWhiteSpace(inquiry.DateOrGuests) ? "" : $"<div style='margin-top: 10px; font-size: 12px; color: {TextMuted};'><strong>Preferred Timing / Group:</strong> {inquiry.DateOrGuests}</div>")}
                        </td>
                    </tr>
                </table>

                <p style='margin: 0 0 20px 0; font-size: 14px; color: {TextCharcoal}; line-height: 1.7;'>
                    One of our senior destination designers is preparing a personalized recommendation tailored to your schedule. We typically reply within 2 to 4 business hours.
                </p>
            </td>
        </tr>");

        sb.Append(GetEmailFooter());
        return sb.ToString();
    }

    public static string BuildAdminInquiryReplyHtml(ContactInquiry inquiry, string replyMessage)
    {
        var sb = new StringBuilder();
        sb.Append(GetEmailHeader(
            preheader: $"Seadora Travel has replied to your travel inquiry.",
            statusPill: "Concierge Response",
            title: "Response to your Inquiry — Seadora Travel",
            subtitle: "Personalized Travel Curation"
        ));

        sb.Append($@"
        <!-- Body Content -->
        <tr>
            <td class='mobile-p-20' style='padding: 36px 36px 28px 36px;'>
                <p style='margin: 0 0 8px 0; font-size: 13px; font-weight: 600; color: {GoldAccent}; text-transform: uppercase; letter-spacing: 1.5px;'>
                    Seadora Concierge Team
                </p>
                <h2 style='margin: 0 0 16px 0; font-family: ""Playfair Display"", Georgia, serif; font-size: 22px; font-weight: 700; color: {NavyDark}; line-height: 1.3;'>
                    Dear {inquiry.FullName},
                </h2>
                <div style='margin: 0 0 24px 0; font-size: 14px; color: {TextCharcoal}; line-height: 1.7; background-color: #FAF8F5; border-left: 4px solid {GoldAccent}; padding: 20px; border-radius: 8px;'>
                    {replyMessage.Replace("\n", "<br/>")}
                </div>

                <!-- Original Inquiry Reference -->
                <div style='border-top: 1px solid {CardBorder}; padding-top: 18px; margin-bottom: 20px;'>
                    <span style='font-size: 11px; font-weight: 600; color: {TextMuted}; text-transform: uppercase; letter-spacing: 0.5px;'>Regarding your original message:</span>
                    <p style='margin: 6px 0 0 0; font-size: 12px; color: {TextMuted}; font-style: italic;'>
                        ""{inquiry.Message}""
                    </p>
                </div>
            </td>
        </tr>");

        sb.Append(GetEmailFooter());
        return sb.ToString();
    }
}
