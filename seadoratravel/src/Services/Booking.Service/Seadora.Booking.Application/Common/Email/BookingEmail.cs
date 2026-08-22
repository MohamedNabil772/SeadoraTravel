using System;
using Seadora.Booking.Domain.Entities;

namespace Seadora.Booking.Application.Common.Email;

public static class BookingEmail
{
    public static string BuildReceiptHtml(Domain.Entities.Booking booking)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #007bff; color: white; padding: 15px; text-align: center; }}
                    .content {{ padding: 20px 0; }}
                    .footer {{ text-align: center; font-size: 12px; color: #777; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Booking Received</h2>
                    </div>
                    <div class='content'>
                        <p>Dear {booking.CustomerName},</p>
                        <p>We have successfully received your booking request for the tour on {(booking.TourDate.HasValue ? booking.TourDate.Value.ToString("d") : "TBD")}.</p>
                        <p><strong>Total Amount:</strong> {booking.TotalPrice:C}</p>
                        <p><strong>Guests:</strong> {booking.Guests}</p>
                        <p>We will review your booking and send you a confirmation shortly.</p>
                        <p>If you have any questions, please contact us at {ContactChannels.InfoEmail}.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.UtcNow.Year} {ContactChannels.DefaultSenderName}. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
    }

    public static string BuildConfirmationHtml(Domain.Entities.Booking booking)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #28a745; color: white; padding: 15px; text-align: center; }}
                    .content {{ padding: 20px 0; }}
                    .policy {{ background-color: #f8f9fa; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                    .footer {{ text-align: center; font-size: 12px; color: #777; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>Booking Confirmed!</h2>
                    </div>
                    <div class='content'>
                        <p>Dear {booking.CustomerName},</p>
                        <p>Great news! Your booking for the tour on {(booking.TourDate.HasValue ? booking.TourDate.Value.ToString("d") : "TBD")} has been officially <strong>confirmed</strong>.</p>
                        <p><strong>Pickup Time:</strong> {(string.IsNullOrEmpty(booking.PickupTime) ? "To be communicated" : booking.PickupTime)}</p>
                        
                        <div class='policy'>
                            <h4>Cancellation Policy</h4>
                            <p>Cancellations made 48 hours or more in advance of the tour date will receive a 100% refund. Cancellations made within 48 hours will incur a 100% fee.</p>
                        </div>
                        
                        <p>We look forward to hosting you!</p>
                        <p>If you have any questions, please contact us at {ContactChannels.InfoEmail}.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.UtcNow.Year} {ContactChannels.DefaultSenderName}. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
    }
}
