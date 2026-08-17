ALTER TABLE "Tours" ADD COLUMN IF NOT EXISTS "Addons" jsonb DEFAULT '[]'::jsonb;
