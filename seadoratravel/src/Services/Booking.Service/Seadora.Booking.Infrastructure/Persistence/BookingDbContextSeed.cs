using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Domain.Entities;
using Seadora.Booking.Domain.Enums;

namespace Seadora.Booking.Infrastructure.Persistence;

public static class BookingDbContextSeed
{
    public static async Task SeedAsync(BookingDbContext context)
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch { }

        await context.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""Notifications"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""Title"" text NOT NULL,
                ""Message"" text NOT NULL,
                ""Type"" text NOT NULL,
                ""ReferenceId"" text,
                ""MetadataJson"" text,
                ""IsRead"" boolean NOT NULL DEFAULT FALSE,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                ""ReadAt"" timestamp with time zone
            );

            CREATE TABLE IF NOT EXISTS ""ContactInquiries"" (
                ""Id"" uuid NOT NULL PRIMARY KEY,
                ""FullName"" text NOT NULL,
                ""Email"" text NOT NULL,
                ""Phone"" text,
                ""DestinationInterest"" text,
                ""DateOrGuests"" text,
                ""Message"" text NOT NULL,
                ""Status"" integer NOT NULL DEFAULT 0,
                ""AdminNotes"" text,
                ""ReplyMessage"" text,
                ""RepliedAt"" timestamp with time zone,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                ""UpdatedAt"" timestamp with time zone
            );

            ALTER TABLE ""ContactInquiries"" ADD COLUMN IF NOT EXISTS ""ReplyMessage"" text;
            ALTER TABLE ""ContactInquiries"" ADD COLUMN IF NOT EXISTS ""RepliedAt"" timestamp with time zone;
            ALTER TABLE ""ContactInquiries"" ADD COLUMN IF NOT EXISTS ""AdminNotes"" text;
            ALTER TABLE ""ContactInquiries"" ADD COLUMN IF NOT EXISTS ""DestinationInterest"" text;
            ALTER TABLE ""ContactInquiries"" ADD COLUMN IF NOT EXISTS ""DateOrGuests"" text;

            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""CustomerName"" text DEFAULT '';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""CustomerEmail"" text DEFAULT '';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""WhatsApp"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""HotelName"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""RoomNumber"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""PassportFileName"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""TripType"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""BookingDate"" timestamp with time zone DEFAULT now();
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""Status"" integer DEFAULT 0;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""IsPaid"" boolean DEFAULT false;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""Attendance"" text DEFAULT 'Pending';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""TourDate"" timestamp with time zone NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""PickupTime"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""Guests"" integer DEFAULT 1;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""HotelPickup"" boolean DEFAULT false;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""PackageId"" uuid NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""TotalPrice"" numeric DEFAULT 0;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""Language"" text DEFAULT 'en';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""SelectedAddons"" jsonb DEFAULT '[]'::jsonb;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""GuestsList"" jsonb DEFAULT '[]'::jsonb;
        ");

        if (await context.Feedbacks.AnyAsync()) return;

        var feedbacks = new List<Feedback>
        {
            // Tour 1
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"), CustomerName = "Emma Watson", CustomerEmail = "emma@example.com", Rating = 5, Comment = "Absolutely spectacular experience at the Pyramids! The guide was very knowledgeable.", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            // Tour 2
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"), CustomerName = "Hans Müller", CustomerEmail = "hans@example.de", Rating = 5, Comment = "Das Grand Egyptian Museum ist atemberaubend. Perfekt organisiert.", CreatedAt = DateTime.UtcNow.AddDays(-15) },
            // Tour 3
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"), CustomerName = "Marco Rossi", CustomerEmail = "marco@example.it", Rating = 4, Comment = "Luxor è magnifica. Un tuffo nel passato. Molto consigliato.", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            // Tour 4
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000104"), CustomerName = "Sophie Dubois", CustomerEmail = "sophie@example.fr", Rating = 5, Comment = "Une expérience inoubliable en parachute, vues incroyables sur la mer!", CreatedAt = DateTime.UtcNow.AddDays(-25) },
            // Tour 5
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000105"), CustomerName = "Alexander Ivanov", CustomerEmail = "alex@example.ru", Rating = 5, Comment = "Сафари на квадроциклах - это круто! Отличный адреналин.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            // Tour 6
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000106"), CustomerName = "Laura Schmidt", CustomerEmail = "laura@example.de", Rating = 4, Comment = "Schöne Aussicht auf die Korallenriffe vom U-Boot aus.", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            // Tour 7
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000107"), CustomerName = "Antonio Conti", CustomerEmail = "antonio@example.it", Rating = 5, Comment = "Orange Bay è un paradiso. Mare cristallino e relax totale.", CreatedAt = DateTime.UtcNow.AddDays(-12) },
            // Tour 8
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000108"), CustomerName = "Elena Volkova", CustomerEmail = "elena@example.ru", Rating = 5, Comment = "Плавание с дельфинами - мечта! Спасибо за этот день.", CreatedAt = DateTime.UtcNow.AddDays(-8) },
            // Tour 9
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000109"), CustomerName = "Chloe Martin", CustomerEmail = "chloe@example.fr", Rating = 5, Comment = "Le vol en montgolfière au lever du soleil sur Louxor est magique.", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            // Tour 10
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000110"), CustomerName = "Oliver Smith", CustomerEmail = "oliver@example.co.uk", Rating = 4, Comment = "Abu Simbel is massive and impressive. A long trip but worth it.", CreatedAt = DateTime.UtcNow.AddDays(-40) },
            // Tour 11
            new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000111"), CustomerName = "Julia Wagner", CustomerEmail = "julia@example.de", Rating = 5, Comment = "Hula Hula Island war sehr entspannend. Tolles Essen und Service.", CreatedAt = DateTime.UtcNow.AddDays(-3) }
        };

        var bookings = new List<Seadora.Booking.Domain.Entities.Booking>
        {
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                CustomerName = "Emma Watson", CustomerEmail = "emma@example.com", WhatsApp = "+44123456789",
                BookingDate = DateTime.UtcNow.AddDays(-30), Status = BookingStatus.Completed, TourDate = DateTime.UtcNow.AddDays(-15),
                TotalPrice = 150,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "VIP Transfer", UnitPrice = 30, Quantity = 1 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                CustomerName = "Hans Müller", CustomerEmail = "hans@example.de", WhatsApp = "+49123456789",
                BookingDate = DateTime.UtcNow.AddDays(-2), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(10),
                TotalPrice = 120,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Lunch Upgrade", UnitPrice = 20, Quantity = 2 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                CustomerName = "Marco Rossi", CustomerEmail = "marco@example.it", WhatsApp = "+39123456789",
                BookingDate = DateTime.UtcNow, Status = BookingStatus.Pending, TourDate = DateTime.UtcNow.AddDays(5),
                TotalPrice = 100
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000104"),
                CustomerName = "Sophie Dubois", CustomerEmail = "sophie@example.fr", WhatsApp = "+33123456789",
                BookingDate = DateTime.UtcNow.AddDays(-5), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(15),
                TotalPrice = 80,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Photo Package", UnitPrice = 15, Quantity = 1 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000105"),
                CustomerName = "Alexander Ivanov", CustomerEmail = "alex@example.ru", WhatsApp = "+79123456789",
                BookingDate = DateTime.UtcNow.AddDays(-20), Status = BookingStatus.Completed, TourDate = DateTime.UtcNow.AddDays(-5),
                TotalPrice = 95,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Scarf & Goggles", UnitPrice = 10, Quantity = 2 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000106"),
                CustomerName = "Laura Schmidt", CustomerEmail = "laura@example.de", WhatsApp = "+49987654321",
                BookingDate = DateTime.UtcNow.AddDays(-50), Status = BookingStatus.Cancelled, TourDate = DateTime.UtcNow.AddDays(-40),
                TotalPrice = 50
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000107"),
                CustomerName = "Antonio Conti", CustomerEmail = "antonio@example.it", WhatsApp = "+39987654321",
                BookingDate = DateTime.UtcNow.AddDays(-3), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(25),
                TotalPrice = 180,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Private Cabana", UnitPrice = 50, Quantity = 1 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000108"),
                CustomerName = "Elena Volkova", CustomerEmail = "elena@example.ru", WhatsApp = "+79987654321",
                BookingDate = DateTime.UtcNow.AddDays(-10), Status = BookingStatus.Completed, TourDate = DateTime.UtcNow.AddDays(-2),
                TotalPrice = 140,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Snorkeling Gear", UnitPrice = 10, Quantity = 3 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000109"),
                CustomerName = "Chloe Martin", CustomerEmail = "chloe@example.fr", WhatsApp = "+33987654321",
                BookingDate = DateTime.UtcNow.AddDays(-12), Status = BookingStatus.Completed, TourDate = DateTime.UtcNow.AddDays(-1),
                TotalPrice = 200
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000110"),
                CustomerName = "Oliver Smith", CustomerEmail = "oliver@example.co.uk", WhatsApp = "+44987654321",
                BookingDate = DateTime.UtcNow.AddDays(-60), Status = BookingStatus.Completed, TourDate = DateTime.UtcNow.AddDays(-45),
                TotalPrice = 150
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000111"),
                CustomerName = "Julia Wagner", CustomerEmail = "julia@example.de", WhatsApp = "+49112233445",
                BookingDate = DateTime.UtcNow.AddDays(1), Status = BookingStatus.Pending, TourDate = DateTime.UtcNow.AddDays(20),
                TotalPrice = 110
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                CustomerName = "Lucas Silva", CustomerEmail = "lucas@example.br", WhatsApp = "+5511999999999",
                BookingDate = DateTime.UtcNow.AddDays(-2), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(30),
                TotalPrice = 160,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Professional Photographer", UnitPrice = 40, Quantity = 1 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"),
                CustomerName = "Maria Garcia", CustomerEmail = "maria@example.es", WhatsApp = "+34666555444",
                BookingDate = DateTime.UtcNow.AddDays(-1), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(40),
                TotalPrice = 130,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "VIP Transfer", UnitPrice = 30, Quantity = 1 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"),
                CustomerName = "Ahmed Hassan", CustomerEmail = "ahmed@example.eg", WhatsApp = "+201012345678",
                BookingDate = DateTime.UtcNow.AddDays(-15), Status = BookingStatus.Cancelled, TourDate = DateTime.UtcNow.AddDays(-5),
                TotalPrice = 90
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000107"),
                CustomerName = "Yuki Tanaka", CustomerEmail = "yuki@example.jp", WhatsApp = "+819012345678",
                BookingDate = DateTime.UtcNow.AddDays(-8), Status = BookingStatus.Confirmed, TourDate = DateTime.UtcNow.AddDays(12),
                TotalPrice = 200,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Seafood Lunch", UnitPrice = 25, Quantity = 2 } }
            },
            new() {
                Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000108"),
                CustomerName = "Chen Wei", CustomerEmail = "chen@example.cn", WhatsApp = "+8613800138000",
                BookingDate = DateTime.UtcNow.AddDays(-4), Status = BookingStatus.Pending, TourDate = DateTime.UtcNow.AddDays(8),
                TotalPrice = 150,
                SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "Snorkeling Gear", UnitPrice = 10, Quantity = 1 } }
            }
        };

        context.Feedbacks.AddRange(feedbacks);
        context.Bookings.AddRange(bookings);
        
        if (!await context.ContactInquiries.AnyAsync())
        {
            var inquiry1 = new ContactInquiry(
                "Lord Alistair Sterling",
                "sterling@luxurytravel.co.uk",
                "+447911123456",
                "Luxury Red Sea Cruise",
                "Nov 15, 4 Guests",
                "We require a private yacht charter with full butler service and diving instructor."
            );
            var inquiry2 = new ContactInquiry(
                "Princess Sarah Al-Saud",
                "sarah.concierge@saudiroyal.org",
                "+966500123456",
                "Bespoke Nile River Elegance",
                "Dec 20, 8 Guests",
                "Requesting private dahabiya sailing between Luxor and Aswan with private Egyptologist."
            );
            context.ContactInquiries.AddRange(inquiry1, inquiry2);

            context.Notifications.AddRange(
                Notification.CreateInquiryNotification(inquiry1.Id, inquiry1.FullName, inquiry1.DestinationInterest ?? "VIP Experience", inquiry1.Email),
                Notification.CreateInquiryNotification(inquiry2.Id, inquiry2.FullName, inquiry2.DestinationInterest ?? "Bespoke Journey", inquiry2.Email),
                Notification.CreateBookingNotification(Guid.NewGuid(), "SEA-782910", "Hans Müller", "Grand Egyptian Museum & Pyramids VIP", 240)
            );
        }

        await context.SaveChangesAsync();
    }
}
