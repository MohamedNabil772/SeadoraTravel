ALTER TABLE "Bookings" ADD COLUMN IF NOT EXISTS "GuestsList" jsonb DEFAULT '[]'::jsonb;
