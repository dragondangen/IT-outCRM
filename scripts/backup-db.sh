#!/bin/bash
# PostgreSQL backup script for IT-outCRM
# Usage: ./backup-db.sh
# Cron:  0 3 * * * /opt/it-outcrm/scripts/backup-db.sh >> /var/log/crm-backup.log 2>&1

set -euo pipefail

BACKUP_DIR="/opt/it-outcrm/backups"
CONTAINER_NAME="itoutcrm-postgres"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/crm_backup_${TIMESTAMP}.sql.gz"
RETENTION_DAYS=14

mkdir -p "$BACKUP_DIR"

echo "[$(date)] Starting PostgreSQL backup..."

docker exec "$CONTAINER_NAME" pg_dumpall -U "$POSTGRES_USER" | gzip > "$BACKUP_FILE"

FILESIZE=$(stat -c%s "$BACKUP_FILE" 2>/dev/null || stat -f%z "$BACKUP_FILE")
echo "[$(date)] Backup created: $BACKUP_FILE ($FILESIZE bytes)"

echo "[$(date)] Removing backups older than ${RETENTION_DAYS} days..."
find "$BACKUP_DIR" -name "crm_backup_*.sql.gz" -mtime +${RETENTION_DAYS} -delete

BACKUP_COUNT=$(find "$BACKUP_DIR" -name "crm_backup_*.sql.gz" | wc -l)
echo "[$(date)] Backup completed. Total backups: $BACKUP_COUNT"
