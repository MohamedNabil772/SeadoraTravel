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
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""MissingIdentification"" boolean DEFAULT false;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""SelectedAddons"" jsonb DEFAULT '[]'::jsonb;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""GuestsList"" jsonb DEFAULT '[]'::jsonb;
        ");

        if (!await context.Feedbacks.AnyAsync())
        {
            var feedbacks = new List<Feedback>
            {
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"), CustomerName = "Emma Watson", CustomerEmail = "emma@example.com", Rating = 5, Comment = "Absolutely spectacular experience at the Pyramids! The guide was very knowledgeable.", CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000102"), CustomerName = "Hans Müller", CustomerEmail = "hans@example.de", Rating = 5, Comment = "Das Grand Egyptian Museum ist atemberaubend. Perfekt organisiert.", CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000103"), CustomerName = "Marco Rossi", CustomerEmail = "marco@example.it", Rating = 4, Comment = "Luxor è magnifica. Un tuffo nel passato. Molto consigliato.", CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000104"), CustomerName = "Sophie Dubois", CustomerEmail = "sophie@example.fr", Rating = 5, Comment = "Une expérience inoubliable en parachute, vues incroyables sur la mer!", CreatedAt = DateTime.UtcNow.AddDays(-25) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000105"), CustomerName = "Alexander Ivanov", CustomerEmail = "alex@example.ru", Rating = 5, Comment = "Сафари на квадроциклах - это круто! Отличный адреналин.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000106"), CustomerName = "Laura Schmidt", CustomerEmail = "laura@example.de", Rating = 4, Comment = "Schöne Aussicht auf die Korallenriffe vom U-Boot aus.", CreatedAt = DateTime.UtcNow.AddDays(-30) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000107"), CustomerName = "Antonio Conti", CustomerEmail = "antonio@example.it", Rating = 5, Comment = "Orange Bay è un paradiso. Mare cristallino e relax totale.", CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000108"), CustomerName = "Elena Volkova", CustomerEmail = "elena@example.ru", Rating = 5, Comment = "Плавание с дельфинами - мечта! Спасибо за этот день.", CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000109"), CustomerName = "Chloe Martin", CustomerEmail = "chloe@example.fr", Rating = 5, Comment = "Le vol en montgolfière au lever du soleil sur Louxor est magique.", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000110"), CustomerName = "Oliver Smith", CustomerEmail = "oliver@example.co.uk", Rating = 4, Comment = "Abu Simbel is massive and impressive. A long trip but worth it.", CreatedAt = DateTime.UtcNow.AddDays(-40) },
                new() { Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000111"), CustomerName = "Julia Wagner", CustomerEmail = "julia@example.de", Rating = 5, Comment = "Hula Hula Island war sehr entspannend. Tolles Essen und Service.", CreatedAt = DateTime.UtcNow.AddDays(-3) }
            };
            context.Feedbacks.AddRange(feedbacks);
        }

        if (!await context.Bookings.AnyAsync())
        {
            var bookings = new List<Seadora.Booking.Domain.Entities.Booking>
            {
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000101"),
                    CustomerName = "Lord Arthur Wellesley", CustomerEmail = "arthur.wellesley@london.co.uk", WhatsApp = "+447700900123",
                    BookingDate = DateTime.UtcNow.AddDays(-14), Status = BookingStatus.Confirmed, IsPaid = true, TourDate = DateTime.UtcNow.AddDays(7),
                    TotalPrice = 320, HotelPickup = true, HotelName = "The Oberoi Sahl Hasheesh", RoomNumber = "104",
                    Guests = 2, MissingIdentification = false,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Lord Arthur Wellesley", Nationality = "United Kingdom", PassportNumber = "GB8829104", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Lady Catherine Wellesley", Nationality = "United Kingdom", PassportNumber = "GB8829105", AgeCategory = "Adult" }
                    },
                    SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "VIP Chauffeur Transfer", UnitPrice = 50, Quantity = 1 } }
                },
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000107"),
                    CustomerName = "Dr. Maximilian Weber", CustomerEmail = "m.weber@munich-med.de", WhatsApp = "+491701234567",
                    BookingDate = DateTime.UtcNow.AddDays(-4), Status = BookingStatus.Confirmed, IsPaid = true, TourDate = DateTime.UtcNow.AddDays(12),
                    TotalPrice = 450, HotelPickup = true, HotelName = "Steigenberger ALDAU Beach Hotel", RoomNumber = "512",
                    Guests = 3, MissingIdentification = false,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Dr. Maximilian Weber", Nationality = "Germany", PassportNumber = "C39018471", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Helga Weber", Nationality = "Germany", PassportNumber = "C39018472", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Lukas Weber", Nationality = "Germany", PassportNumber = "C39018473", AgeCategory = "Child" }
                    },
                    SelectedAddons = new List<BookingAddonSnapshot> { new() { AddonId = Guid.NewGuid(), Title = "VIP Private Cabana & Seafood Lunch", UnitPrice = 75, Quantity = 1 } }
                },
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000108"),
                    CustomerName = "Elena Rostova", CustomerEmail = "elena.rostova@vip-concierge.ru", WhatsApp = "+79161234567",
                    BookingDate = DateTime.UtcNow.AddDays(-1), Status = BookingStatus.Pending, IsPaid = false, TourDate = DateTime.UtcNow.AddDays(5),
                    TotalPrice = 280, HotelPickup = true, HotelName = "Rixos Premium Magawish", RoomNumber = "Villa 12",
                    Guests = 2, MissingIdentification = true,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Elena Rostova", Nationality = "Russia", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Dmitry Rostov", Nationality = "Russia", AgeCategory = "Adult" }
                    }
                },
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000105"),
                    CustomerName = "Countess Isabella Moretti", CustomerEmail = "isabella.moretti@milano.it", WhatsApp = "+393401234567",
                    BookingDate = DateTime.UtcNow.AddDays(-8), Status = BookingStatus.Completed, IsPaid = true, TourDate = DateTime.UtcNow.AddDays(-2),
                    TotalPrice = 190, HotelPickup = true, HotelName = "Baron Palace Sahl Hasheesh", RoomNumber = "302",
                    Guests = 2, MissingIdentification = false,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Countess Isabella Moretti", Nationality = "Italy", PassportNumber = "YA9920141", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Gianluca Moretti", Nationality = "Italy", PassportNumber = "YA9920142", AgeCategory = "Adult" }
                    }
                },
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000109"),
                    CustomerName = "Jean-Pierre Laurent", CustomerEmail = "jp.laurent@paris-luxury.fr", WhatsApp = "+33612345678",
                    BookingDate = DateTime.UtcNow.AddDays(-6), Status = BookingStatus.Confirmed, IsPaid = true, TourDate = DateTime.UtcNow.AddDays(15),
                    TotalPrice = 600, HotelPickup = true, HotelName = "Kempinski Hotel Soma Bay", RoomNumber = "Lagoon Suite 4",
                    Guests = 2, MissingIdentification = false,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Jean-Pierre Laurent", Nationality = "France", PassportNumber = "18AF90214", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Claire Laurent", Nationality = "France", PassportNumber = "18AF90215", AgeCategory = "Adult" }
                    }
                },
                new() {
                    Id = Guid.NewGuid(), TourId = Guid.Parse("00000000-0000-0000-0000-000000000104"),
                    CustomerName = "Sir Henry Sterling", CustomerEmail = "sterling.h@edinburgh.ac.uk", WhatsApp = "+447800112233",
                    BookingDate = DateTime.UtcNow.AddDays(-2), Status = BookingStatus.Pending, IsPaid = false, TourDate = DateTime.UtcNow.AddDays(9),
                    TotalPrice = 180, HotelPickup = true, HotelName = "Albatros White Beach", RoomNumber = "208",
                    Guests = 2, MissingIdentification = true,
                    GuestsList = new List<GuestDetail> {
                        new() { Id = Guid.NewGuid(), FullName = "Sir Henry Sterling", Nationality = "United Kingdom", AgeCategory = "Adult" },
                        new() { Id = Guid.NewGuid(), FullName = "Margaret Sterling", Nationality = "United Kingdom", AgeCategory = "Adult" }
                    }
                }
            };

            context.Bookings.AddRange(bookings);
        }
        
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
