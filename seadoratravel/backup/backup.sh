#!/bin/sh
# Daily backup of Postgres (all DBs) + uploaded files into /backups.
# Runs as a sidecar on the postgres:15-alpine image (pg_dumpall/tar/gzip built in).
# ponytail: dump-based backups; switch to WAL/PITR only if RPO must be < 1 day.
set -eu

DIR=/backups
RETENTION_DAYS="${BACKUP_RETENTION_DAYS:-7}"
INTERVAL="${BACKUP_INTERVAL:-86400}"   # seconds between backups (default: daily)

run_once() {
  mkdir -p "$DIR"
  ts=$(date +%Y%m%d-%H%M%S)
  echo "[backup] $ts starting"
  # All databases + roles in one restorable file.
  pg_dumpall -h postgres -U postgres | gzip > "$DIR/db-$ts.sql.gz"
  # Uploaded files (empty dir is fine).
  tar czf "$DIR/uploads-$ts.tar.gz" -C /data/uploads . 2>/dev/null || true
  # Retention: drop archives older than RETENTION_DAYS.
  find "$DIR" -type f -name '*.gz' -mtime +"$RETENTION_DAYS" -delete
  echo "[backup] $ts done"
}

# `backup once` runs a single cycle and exits (used as the runnable self-check).
if [ "${1:-}" = "once" ]; then
  run_once
  exit 0
fi

while true; do
  run_once
  sleep "$INTERVAL"
done
