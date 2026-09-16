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
                ""Status"" text NOT NULL DEFAULT 'Pending',
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

            -- Repair: ContactInquiries.Status is mapped as string (HasConversion<string>) but older
            -- deployments created the column as integer, which breaks every insert. Convert in place.
            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM information_schema.columns
                           WHERE table_name = 'ContactInquiries' AND column_name = 'Status'
                           AND data_type = 'integer') THEN
                    ALTER TABLE ""ContactInquiries"" ALTER COLUMN ""Status"" DROP DEFAULT;
                    ALTER TABLE ""ContactInquiries"" ALTER COLUMN ""Status"" TYPE text USING (
                        CASE ""Status""
                            WHEN 0 THEN 'Pending' WHEN 1 THEN 'Replied'
                            WHEN 2 THEN 'Resolved' WHEN 3 THEN 'Archived'
                            ELSE 'Pending' END);
                    ALTER TABLE ""ContactInquiries"" ALTER COLUMN ""Status"" SET DEFAULT 'Pending';
                END IF;
            END $$;

            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""CustomerName"" text DEFAULT '';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""CustomerEmail"" text DEFAULT '';
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""WhatsApp"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""HotelName"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""RoomNumber"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""PassportFileName"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""TripType"" text NULL;
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""BookingDate"" timestamp with time zone DEFAULT now();
            ALTER TABLE ""Bookings"" ADD COLUMN IF NOT EXISTS ""Status"" text DEFAULT 'Pending';
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

        await context.SaveChangesAsync();
    }
}
