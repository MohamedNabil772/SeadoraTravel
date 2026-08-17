ALTER TABLE "Bookings" ADD COLUMN IF NOT EXISTS "SelectedAddons" jsonb DEFAULT '[]'::jsonb;
